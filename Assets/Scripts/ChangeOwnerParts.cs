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

    private NetworkVariable<float> velocityNetworkX =
        new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> velocityNetworkY =
        new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> velocityNetworkZ =
        new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);

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
            velocityNetworkX.Value = rd.velocity.x;
            velocityNetworkY.Value = rd.velocity.y;
            velocityNetworkZ.Value = rd.velocity.z;
        }
        
        velocityNetworkX.OnValueChanged += ChangeVelocityX;
        velocityNetworkY.OnValueChanged += ChangeVelocityY;
        velocityNetworkZ.OnValueChanged += ChangeVelocityZ;
    }

    public override void OnNetworkDespawn()
    {
        velocityNetworkX.OnValueChanged -= ChangeVelocityX;
        velocityNetworkY.OnValueChanged -= ChangeVelocityY;
        velocityNetworkZ.OnValueChanged -= ChangeVelocityZ;
    }

    private void ChangeVelocityX(float previousvalue, float newvalue)
    {
        SetRigidbodyVelocity(new Vector3(newvalue, rd.velocity.y, rd.velocity.z));
    }
    
    private void ChangeVelocityY(float previousvalue, float newvalue)
    {
        SetRigidbodyVelocity(new Vector3(rd.velocity.x, newvalue, rd.velocity.z));
    }
    
    private void ChangeVelocityZ(float previousvalue, float newvalue)
    {
        SetRigidbodyVelocity(new Vector3(rd.velocity.x, rd.velocity.y, newvalue));
    }

    private void SetRigidbodyVelocity(Vector3 newVelocity)
    {
        if (IsOwner)
            return;
        
        rd.velocity = newVelocity;
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

            VelocityComparison(rd.velocity.x, ref velocityNetworkX);
            VelocityComparison(rd.velocity.y, ref velocityNetworkY);
            VelocityComparison(rd.velocity.z, ref velocityNetworkZ);

            await UniTask.DelayFrame(20, cancellationToken: token);
        }
    }
    
    private void VelocityComparison(float newVelocity, ref NetworkVariable<float> checkVelocity)
    {
        // if (!Mathf.Approximately(newVelocity, checkVelocity.Value))
        //     checkVelocity.Value = newVelocity;
        
        //自分でチェックしなくても勝手にやってくれてた
        checkVelocity.Value = newVelocity;
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
            
            if(IsOwner)
            {
                OwnerRequestServerRpc(playerManagement.playerArray[playerIndex].OwnerClientId);
            }
        }
    }
}
