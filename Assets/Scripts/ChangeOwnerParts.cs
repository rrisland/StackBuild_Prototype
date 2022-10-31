using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


public class ChangeOwnerParts : NetworkBehaviour
{
    [SerializeField] private PlayerManagement playerManagement;
    private NetworkVariable<Vector3> velocityNetwork = 
        new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkObject networkObject;

    private Rigidbody rd;

    private bool isSetVelocity = true;
    private Vector3 networkVelocity = Vector3.zero;

    [ServerRpc(RequireOwnership = false)]
    private void OwnerRequestServerRpc(ulong cloentId)
    {
        networkObject.ChangeOwnership(cloentId);
    }

    //クライアントからVelocityを受け取る
    [ServerRpc(RequireOwnership = false)]
    private void VelocityServerRpc(Vector3 velocity)
    {
        isSetVelocity = false;
        networkVelocity = velocity;
    }

    private void Awake()
    {
        TryGetComponent(out networkObject);
        TryGetComponent(out rd);
    }


    private void Start()
    {
        CheckDistance(this.GetCancellationTokenOnDestroy()).Forget();
        RigidbodySync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            velocityNetwork.Value = rd.velocity;
        }
        else
        {
            //クライアントがホストから受け取る場合
            velocityNetwork.OnValueChanged += ChangeRigidbody;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
            velocityNetwork.OnValueChanged -= ChangeRigidbody;
    }

    //クライアントがホストから情報を受け取り
    private void ChangeRigidbody(Vector3 previousvalue, Vector3 newvalue)
    {
        isSetVelocity = false;
        networkVelocity = newvalue;
    }

    public override void OnGainedOwnership()
    {
        isSetVelocity = true;
        rd.velocity = networkVelocity;
    }

    private async UniTask RigidbodySync(CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested();

            if (!IsOwner)
            {
                await UniTask.Yield(cancellationToken: token);
                continue;
            }

            if(IsServer)
            {
                velocityNetwork.Value = rd.velocity;
            }
            else
            {
                VelocityServerRpc(rd.velocity);
            }

            await UniTask.DelayFrame(50, cancellationToken: token);
        }
    }

    private async UniTask CheckDistance(CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested();

            if (!IsOwner)
            {
                await UniTask.Yield(cancellationToken: token);
                continue;
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            var p1 = Vector3.Distance(transform.position, playerManagement.playerArray[0].transform.position);
            var p2 = Vector3.Distance(transform.position, playerManagement.playerArray[1].transform.position);

            //近いほうのインデックスを入れる
            int playerIndex = p1 > p2 ? 1 : 0;

            //自分と変更先が一致している場合は何もしない
            if (playerManagement.playerArray[playerIndex].OwnerClientId == OwnerClientId)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
                continue;
            }

            rd.velocity = Vector3.zero;
            rd.angularVelocity = Vector3.zero;
            OwnerRequestServerRpc(playerManagement.playerArray[playerIndex].OwnerClientId);
        }
    }
}
