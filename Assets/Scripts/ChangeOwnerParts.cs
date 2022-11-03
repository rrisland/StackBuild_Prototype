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
    
    private Vector3 beforeVelocity = Vector3.zero;
    private NetworkVariable<Vector3> velocityNetwork = 
        new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkObject networkObject;

    private Rigidbody rd;

    [ServerRpc]
    private void OwnerRequestServerRpc(ulong cloentId)
    {
        networkObject.ChangeOwnership(cloentId);
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
        if (IsOwner)
        {
            velocityNetwork.Value = rd.velocity;
        }
        
        velocityNetwork.OnValueChanged += ChangeRigidbodyVelocity;
    }

    public override void OnNetworkDespawn()
    {
        velocityNetwork.OnValueChanged -= ChangeRigidbodyVelocity;
    }

    private void ChangeRigidbodyVelocity(Vector3 previousvalue, Vector3 newvalue)
    {
        if (IsOwner)
            return;
        
        rd.velocity = newvalue;
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

            velocityNetwork.Value = rd.velocity;

            await UniTask.DelayFrame(10, cancellationToken: token);
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
            
            
            OwnerRequestServerRpc(playerManagement.playerArray[playerIndex].OwnerClientId);
        }
    }
}
