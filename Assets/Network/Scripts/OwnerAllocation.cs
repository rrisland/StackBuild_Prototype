using System;
using System.Collections;
using System.Collections.Generic;
using NetworkSystem;
using UniRx;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkSystem
{
    public class OwnerAllocation : MonoBehaviour
    {
        [SerializeField] private NetworkObject[] playerObjects = Array.Empty<NetworkObject>();

        private void Start()
        {
            ObjectOwnerAllocation();
        }

        void ObjectOwnerAllocation()
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            var clients = NetworkManager.Singleton.ConnectedClients;
            int i = 0;
            foreach (var client in clients)
            {
                if (i >= playerObjects.Length)
                    break;

                playerObjects[i].ChangeOwnership(client.Value.ClientId);
                i++;
            }
        }
    }
}
