using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace StackProto
{
    public class PartsCatch : NetworkBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private Cone cone = null;
        [SerializeField] private PlayerData data;

        [SerializeField] private bool IsFirstPlayer = false;

        private Player player;

        private bool isCatchHold = false;

        [ServerRpc]
        public void HoldServerRpc(bool isHold)
        {
            isCatchHold = isHold;
            
            if(!isHold)
                CatchupRelease();
        }

        [ClientRpc]
        public void HoldClientRpc(bool isHold)
        {
            isCatchHold = isHold;
            
            if(!isHold)
                CatchupRelease();
        }

        public override void OnNetworkSpawn()
        {
            TryGetComponent(out player);
            
            if (!IsOwner)
                return;
            
            if (NetworkManager.Singleton.IsServer && 
                NetworkManager.Singleton.ConnectedClients.Count == 1 && !IsFirstPlayer)
                return;

            inputSender.Catch.Subscribe(x =>
            {
                CatchupRelease();
            }).AddTo(this);
        }

        private void Update()
        {
            if (!IsSpawned || !IsOwner)
                return;
            
            if (NetworkManager.Singleton.IsServer && 
                NetworkManager.Singleton.ConnectedClients.Count == 1 && !IsFirstPlayer)
                return;
            
            if(inputSender.Catch.Value)
                CatchupStay();
        }

        private void CatchupStay()
        {
            
            foreach (var rb in cone.innerObjectsRb)
            {
                rb.Value.AddForce(player.velocity * data.moveSpeed, ForceMode.Acceleration);

                var center = cone.gameObject.transform.position;
                var sub = center - rb.Value.transform.position;

                rb.Value.AddForceAtPosition(sub * (data.catchupPower * Time.deltaTime), center, ForceMode.VelocityChange);

                var magnitude = sub.magnitude;
                if (magnitude < data.catchupRange)
                {
                    rb.Value.velocity = rb.Value.velocity * (magnitude / data.catchupRange);
                }
            }
        }

        private void CatchupRelease()
        {
            foreach (var rb in cone.innerObjectsRb)
            {
                var position = transform.position;
                var dest = position + (Vector3.down * position.y);
                var sub = dest - rb.Value.transform.position;

                rb.Value.AddForceAtPosition(sub * (data.releasePower * Time.deltaTime), dest, ForceMode.VelocityChange);
            }
        }
    }

}