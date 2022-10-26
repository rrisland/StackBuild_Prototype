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

    private NetworkObject networkObject;

    [ServerRpc(RequireOwnership = false)]
    public void OwnerRequestServerRpc(ulong cloentId)
    {
        networkObject.ChangeOwnership(cloentId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OwnerRemoveServerRpc()
    {
        networkObject.RemoveOwnership();
    }


    private void Start()
    {
        TryGetComponent(out networkObject);

        CheckDistance(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask CheckDistance(CancellationToken token)
    {
        while (true)
        {
            if (!IsOwner)
            {
                await UniTask.Yield();
                continue;
            }
            
            var p1 = Vector3.Distance(transform.position, playerManagement.playerArray[0].transform.position);
            var p2 = Vector3.Distance(transform.position, playerManagement.playerArray[1].transform.position);

            //近いほうのインデックスを入れる
            int playerIndex = p1 > p2 ? 1 : 0;

            //自分と変更先が一致している場合は何もしない
            if (playerManagement.playerArray[playerIndex].OwnerClientId == OwnerClientId)
            {
                await UniTask.Yield();
                continue;
            }
        
            OwnerRequestServerRpc(playerManagement.playerArray[playerIndex].OwnerClientId);
            
            //交換した場合Dilayする
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
        }
    }
}
