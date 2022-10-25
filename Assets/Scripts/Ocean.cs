using System;
using System.Collections.Generic;
using UnityEngine;

namespace StackProto
{
    [RequireComponent(typeof(MeshCollider))]
    public class Ocean : ParticlePool
    {
        private void OnTriggerExit(Collider other)
        {
            var system = objectPool.Get();
            var pos = other.gameObject.transform.position;
            pos.y = transform.position.y + 0.5f;
            system.gameObject.transform.position = pos;
        }
    }
}
