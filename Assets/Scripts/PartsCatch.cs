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
        [SerializeField] private PlayerData data;

        [SerializeField] private bool IsFirstPlayer = false;

        [SerializeField] private Player player;

        private IDisposable disposable;
        private NetworkVariable<bool> isCatchHold =
            new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);

        public override void OnGainedOwnership()
        {
            disposable = inputSender.Catch.Subscribe(x =>
            {
                if (!IsOwner)
                    return;

                isCatchHold.Value = x;
            });
        }

        public override void OnLostOwnership()
        {
            disposable.Dispose();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!IsSpawned)
                return;
            
            if (NetworkManager.Singleton.IsServer && 
                NetworkManager.Singleton.ConnectedClients.Count == 1 && !IsFirstPlayer)
                return;

            if(isCatchHold.Value && other.TryGetComponent(out Rigidbody rb))
                CatchupStay(rb);
        }

        private void CatchupStay(Rigidbody rb)
        {
            
            rb.AddForce(player.velocity * data.moveSpeed, ForceMode.Acceleration);

            var center = transform.position;
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