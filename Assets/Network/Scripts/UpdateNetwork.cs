using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using UnityEngine;

namespace NetworkSystem
{
    public class UpdateNetwork : MonoBehaviour
    {
        public LobbyManager lobby;
        public RelayManager relay;

        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += x =>
            {
                //ホストの場合ロビーを抜ける
                if (x == 0)
                    NetworkSystemManager.NetworkExit(lobby, relay).Forget();
            };
        }

        private void OnApplicationQuit()
        {
            //ホストの場合
            NetworkSystemManager.NetworkExit(lobby, relay).Forget();
        }
    }
}
