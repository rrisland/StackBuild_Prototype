using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace StackProto
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private InputSender inputSender;
        
        [SerializeField] private PlayerData data;
        [SerializeField] private bool IsFirstPlayer = true;
        [SerializeField] private Cone cone = null;

        private Vector3 velocity = Vector3.zero;
        //private bool isCatchupped = false;

        private void Start()
        {
            if (!IsOwner && !IsFirstPlayer)
                return;

            inputSender.Catch.Where(x => !x).Subscribe(x =>
            {
                CatchupRelease();
            }).AddTo(this);
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (IsServer && NetworkManager.Singleton.ConnectedClients.Count == 1 && !IsFirstPlayer)
                return;

            Vector2 dir = Vector2.zero;

            dir = inputSender.Move.Value;

            // if (Input.GetKey(IsFirstPlayer ? KeyCode.W : KeyCode.I))
            // {
            //     dir += Vector2.up;
            // }
            // if (Input.GetKey(IsFirstPlayer ? KeyCode.S : KeyCode.K))
            // {
            //     dir += Vector2.down;
            // }
            // if (Input.GetKey(IsFirstPlayer ? KeyCode.A : KeyCode.J))
            // {
            //     dir += Vector2.left;
            // }
            // if (Input.GetKey(IsFirstPlayer ? KeyCode.D : KeyCode.L))
            // {
            //     dir += Vector2.right;
            // }

            dir.Normalize();

            velocity *= data.attenuation;
            velocity += new Vector3(dir.x, 0.0f, dir.y) * (data.accelaration * Time.deltaTime);
            if (velocity.magnitude > data.moveSpeed)
            {
                velocity = velocity.normalized * data.moveSpeed;
            }

            
            
            
            var dest = transform.position + velocity;

            if (dest.x < data.playAreaMin.x)
            {
                dest.x = data.playAreaMin.x;
            }
            else if (dest.x > data.playAreaMax.x)
            {
                dest.x = data.playAreaMax.x;
            }
            if (dest.z < data.playAreaMin.y)
            {
                dest.z = data.playAreaMin.y;
            }
            else if (dest.z > data.playAreaMax.y)
            {
                dest.z = data.playAreaMax.y;
            }
            
            transform.position = dest;

            // if (Input.GetKeyDown(IsFirstPlayer ? KeyCode.LeftShift : KeyCode.Space))
            // {
            //     isCatchupped = true;
            // }
            // if (Input.GetKeyUp(IsFirstPlayer ? KeyCode.LeftShift : KeyCode.Space))
            // {
            //     isCatchupped = false;
            //     CatchupRelease();
            // }
            if (inputSender.Catch.Value)
            {
                CatchupStay();
            }
        }

        private void CatchupStay()
        {
            foreach (var rb in cone.innerObjectsRb)
            {
                rb.AddForce(velocity * data.moveSpeed, ForceMode.Acceleration);
                
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

        private void Dash()
        {
            
        }
    }
}
