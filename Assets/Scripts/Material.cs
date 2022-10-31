using Unity.Netcode;
using UnityEngine;

namespace StackProto
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    //[RequireComponent(typeof(Rigidbody))]
    public class Material : MonoBehaviour
    {
        private static MaterialData data;
        public static int InstanceCounter { get; private set; } = 0;

        public int MaterialIndex;
        public int MeshIndex;

        private MeshFilter filter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private Rigidbody rb;

        private void Start()
        {
            filter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            rb = GetComponent<Rigidbody>();

            InstanceCounter++;
        }

        public void OnDestroy()
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

            filter.sharedMesh = mesh;
            
            meshRenderer.sharedMaterial = material;

            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;

            // if (TryGetComponent(out rb))
            // {
            //     rb.interpolation = RigidbodyInterpolation.Interpolate;
            //     rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            // }
        }
        
        void OnTriggerStay(Collider other)
        {
            if (other.transform.parent is null) return;

            var parent = other.transform.parent;

            if (parent.TryGetComponent(out Player player))
            {
                player.Vacuum(rb);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.transform.parent is null) return;
            
            var parent = other.transform.parent;

            if (parent.TryGetComponent(out Player player))
            {
                player.VacuumRelease(rb);
            }
        }
    }
}

