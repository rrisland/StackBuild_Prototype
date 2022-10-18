using System.Collections;
using System.Collections.Generic;
using StackProto;
using UnityEngine;

namespace StackProto
{
    public class CanonCollectArea : MonoBehaviour
    {
        [SerializeField] private CanonQueue queue;
    
        private void OnTriggerEnter(Collider other)
        {
            if (queue is null) return;

            if (other.TryGetComponent(out StackProto.Material material))
            {
                queue.Objects.Enqueue(other.gameObject);
            }
        }
    }
}


