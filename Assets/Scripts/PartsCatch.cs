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

        private Player player;

        private bool isCatchHold = false;

        [ServerRpc]
        public void HoldServerRpc(bool isHold)
        {
            isCatchHold = isHold;
            
            if(!isHold)
                CatchupRelease();
        }

        private void Start()
        {
            TryGetComponent(out player);
            
            if (!IsOwner)
                return;

            inputSender.Catch.Subscribe(x =>
            {
                HoldServerRpc(x);
            }).AddTo(this);
        }

        private void Update()
        {
            if (!IsServer)
                return;
                
            
            if(isCatchHold)
                CatchupStay();
        }

        private void CatchupStay()
        {
            foreach (var rb in cone.innerObjectsRb)
            {
                rb.AddForce(player.velocity * data.moveSpeed, ForceMode.Acceleration);

                var center = cone.gameObject.transform.position;
                var sub = center - rb.transform.position;

                rb.AddForceAtPosition(sub * (data.catchupPower * Time.deltaTime), center, ForceMode.VelocityChange);

                var magnitude = sub.magnitude;
                if (magnitude < data.catchupRange)
                {
                    rb.velocity = rb.velocity * (magnitude / data.catchupRange);
                }
            }
        }

        private void CatchupRelease()
        {
            foreach (var rb in cone.innerObjectsRb)
            {
                var position = transform.position;
                var dest = position + (Vector3.down * position.y);
                var sub = dest - rb.transform.position;

                rb.AddForceAtPosition(sub * (data.releasePower * Time.deltaTime), dest, ForceMode.VelocityChange);
            }
        }
    }

}