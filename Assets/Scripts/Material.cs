using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace StackProto
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Material : MonoBehaviour
    {
        private static MaterialData data;
        public static int InstanceCounter { get; private set; } = 0;

        public int MaterialIndex;
        public int MeshIndex;

        public MeshFilter filter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        public Rigidbody rb;

        private void Start()
        {
            InstanceCounter++;
        }

        private void OnDestroy()
        {
            InstanceCounter--;
        }

        public static void Initialize(MaterialData materialData)
        {
            data = materialData;
        }

        public void SetData(int materialIndex, int meshIndex)
        {
            UnityEngine.Material material = data.materials[materialIndex];
            UnityEngine.Mesh mesh = data.meshes[meshIndex];

            MaterialIndex = materialIndex;
            MeshIndex = meshIndex;

            if (TryGetComponent(out filter))
            {
                filter.sharedMesh = mesh;
            }
            
            if (TryGetComponent(out meshRenderer))
            {
                meshRenderer.sharedMaterial = material;
            }

            if (TryGetComponent(out meshCollider))
            {
                meshCollider.sharedMesh = mesh;
                meshCollider.convex = true;
            }

            if (TryGetComponent(out rb))
            {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }
    }
}

