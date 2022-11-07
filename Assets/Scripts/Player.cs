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
    public class Player : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        
        [SerializeField] private PlayerData data;
        [SerializeField] private bool IsFirstPlayer = true;

        [SerializeField] private PlayerManagement playerManagement;

        private NetworkObject networkObject;
        

        public Vector3 velocity { get; private set; } = Vector3.zero;
        //private bool isCatchupped = false;

        private void Start()
        {
            TryGetComponent(out networkObject);
            
            //プレイヤーセットする
            playerManagement.SetPlayer(IsFirstPlayer ? 0 : 1, networkObject);

            
        }

        private void Update()
        {
            if (!networkObject.IsOwner)
                return;
            
            if (NetworkManager.Singleton.IsServer && 
                NetworkManager.Singleton.ConnectedClients.Count == 1 && !IsFirstPlayer)
                return;

            Vector2 dir = Vector2.zero;

            dir = inputSender.Move.Value;

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
        }
        
        public void Vacuum(Rigidbody rb)
        {
            if (!inputSender.Catch.Value) return;
		
            // 引きよせる
            rb.AddForce(velocity * data.moveSpeed, ForceMode.Acceleration);

            var center = transform.position + Vector3.down * 7.5f;
            var sub = center - rb.transform.position;

            rb.AddForceAtPosition(sub * (data.catchupPower * Time.deltaTime), center, ForceMode.VelocityChange);

            var magnitude = sub.magnitude;
            if (magnitude < data.catchupRange)
            {
                rb.velocity = rb.velocity * (magnitude / data.catchupRange);
            }
        }

        public void VacuumRelease(Rigidbody rb)
        {
            var position = transform.position;
            var dest = position + (Vector3.down * position.y);
            var sub = dest - rb.transform.position;

            rb.AddForceAtPosition(sub * (data.releasePower * Time.deltaTime), dest, ForceMode.VelocityChange);
        }
    }
}
