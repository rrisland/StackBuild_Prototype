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


        [ServerRpc(RequireOwnership = false)]
        public void HoldServerRpc(bool isHold)
        {
            isCatchHold = isHold;
            
            /*
            if(!isHold)
                CatchupRelease();
            */
        }

        [ClientRpc]
        public void HoldClientRpc(bool isHold)
        {
            isCatchHold = isHold;
            
            /*
            if(!isHold)
                CatchupRelease();
            */
        }

        public override void OnNetworkSpawn()
        {
            TryGetComponent(out player);
            
            if (!IsOwner)
                return;
            
            if (NetworkManager.Singleton.IsServer && 
                NetworkManager.Singleton.ConnectedClients.Count == 1 && !IsFirstPlayer)
                return;

            /*
            inputSender.Catch.Subscribe(x =>
            {
                CatchupRelease();
            }).AddTo(this);
            */
        }

        private void OnTriggerStay(Collider other)
        {
            /*
            if (!IsSpawned || !IsOwner)
                return;
            
            if (NetworkManager.Singleton.IsServer && 
                NetworkManager.Singleton.ConnectedClients.Count == 1 && !IsFirstPlayer)
                return;
            */

            if (other.TryGetComponent(out NetworkObject obj))
            {
                if (!obj.IsOwner) return;
            }
            
            if (other.TryGetComponent(out Rigidbody rb))
            {
                if (inputSender.Catch.Value)
                {
                    //CatchupStay(rb);
                }
            }
        }

        private void CatchupStay(Rigidbody rb)
        {
            rb.AddForce(player.velocity * data.moveSpeed, ForceMode.Acceleration);

            var center = cone.transform.position;
            var sub = center - rb.transform.position;

            rb.AddForceAtPosition(sub * (data.catchupPower * Time.deltaTime), center, ForceMode.VelocityChange);

            var magnitude = sub.magnitude;
            if (magnitude < data.catchupRange)
            {
                rb.velocity = rb.velocity * (magnitude / data.catchupRange);
            }
        }

        private void CatchupRelease(Rigidbody rb)
        {
            var position = transform.position;
            var dest = position + (Vector3.down * position.y);
            var sub = dest - rb.transform.position;

            rb.AddForceAtPosition(sub * (data.releasePower * Time.deltaTime), dest, ForceMode.VelocityChange);
        }
    }
}