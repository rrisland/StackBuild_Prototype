using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace StackProto
{
    public class Cone : MonoBehaviour
    {
        //public List<Rigidbody> innerObjectsRb = new List<Rigidbody>();
        public Dictionary<int, Rigidbody> innerObjectsRb = new Dictionary<int, Rigidbody>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out NetworkObject networkObject) && !networkObject.IsOwner)
                return;
            
            if (other.gameObject.TryGetComponent(out Rigidbody rb))
            {
                innerObjectsRb.Add(other.gameObject.GetInstanceID(), rb);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            innerObjectsRb.Remove(other.gameObject.GetInstanceID());
        }
    }
}
