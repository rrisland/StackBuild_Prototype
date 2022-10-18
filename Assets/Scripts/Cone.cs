using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace StackProto
{
    public class Cone : MonoBehaviour
    {
        public List<Rigidbody> innerObjectsRb = new List<Rigidbody>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Rigidbody rb))
            {
                innerObjectsRb.Add(rb);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Rigidbody rb))
            {
                innerObjectsRb.Remove(rb);
            }
        }
    }
}
