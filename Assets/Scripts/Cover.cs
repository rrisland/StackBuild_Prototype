using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Burst;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class Cover : NetworkBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float appearanceTime = 15.0f;
    [SerializeField] private float intervalTime = 5.0f;

    private void Awake()
    {
        if (meshRenderer is null) TryGetComponent(out meshRenderer);
        if (meshCollider is null) TryGetComponent(out meshCollider);
    }
    
    private void Start()
    {
        if (meshRenderer is null || meshCollider is null) return;
        CuverCloseOpen(true);
    }

    public override void OnNetworkSpawn()
    {
        ClientSyncedCuverCloseOpen(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask ClientSyncedCuverCloseOpen(CancellationToken token)
    {
        if (!IsOwner)
            return;

        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(appearanceTime), cancellationToken: token);
            CuverSyncedServerRpc(NetworkManager.LocalTime.Time);
            await UniTask.Delay(TimeSpan.FromSeconds(intervalTime), cancellationToken: token);
            CuverSyncedServerRpc(NetworkManager.LocalTime.Time);
        }
    }
    
    public void CuverCloseOpen(bool isClose)
    {
        meshRenderer.enabled = isClose;
        meshCollider.enabled = isClose;
    }

    private async UniTask WaitAndCuverOpenClose(float timeToWait, CancellationToken token)
    {
        if (timeToWait > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), cancellationToken: token);
        
        CuverCloseOpen(!meshRenderer.enabled);
        //Debug.LogError("Cuver: " + (meshRenderer.enabled ? "閉じてる" : "空いてる") + $"\nTime: {NetworkManager.LocalTime.Time - timeToWait}");
    }

    [ServerRpc]
    private void CuverSyncedServerRpc(double time)
    {
        CuverSyncedClientRpc(time);
        var timeToWait = time - NetworkManager.ServerTime.Time;
        WaitAndCuverOpenClose((float) timeToWait, this.GetCancellationTokenOnDestroy()).Forget();
    }

    [ClientRpc]
    private void CuverSyncedClientRpc(double time)
    {
        if (IsOwner)
            return;

        var timeToWait = time - NetworkManager.ServerTime.Time;
        WaitAndCuverOpenClose((float)timeToWait, this.GetCancellationTokenOnDestroy()).Forget();
    }
    
}
