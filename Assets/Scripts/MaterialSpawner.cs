using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using StackProto;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SocialPlatforms;


namespace StackProto
{
    public class MaterialSpawner : NetworkBehaviour
    {
        [SerializeField] private MaterialData materialData;
        [SerializeField] private MaterialSpawnerData spawnerData;
        [SerializeField] private List<CanonQueue> queues;
        [SerializeField] private GameObject partsPrefub;

        private List<UnityEngine.Material> materials => materialData.materials;
        private List<UnityEngine.Mesh> meshes => materialData.meshes;

        private List<GameObject> partsList = new List<GameObject>();

        public override void OnNetworkSpawn()
        {
            if (!IsServer || materialData is null || queues is null) 
                return;
            
            StackProto.Material.Initialize(materialData);
            StartCoroutine(SpawnTimer());
        }

        IEnumerator SpawnTimer()
        {
            while (true)
            {
                if(StackProto.Material.InstanceCounter <= 30)
                {
                    int index = Random.Range(0, queues.Count);

                    for (int i = 0; i < spawnerData.quantity; i++)
                    {
                        Spawn(out var obj);
                        queues[index].Objects.Enqueue(obj);
                    }
                }
                
                yield return new WaitForSeconds(spawnerData.duration);
            }
        }

        void Spawn(out GameObject obj)
        {
            obj = Instantiate(partsPrefub);
            obj.transform.position = new Vector3(0.0f, -100.0f, 0.0f);

            var netobj = obj.GetComponent<NetworkObject>();
            netobj.Spawn(true);
            netobj.DontDestroyWithOwner = false;
            
            var rb = obj.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            //Material material = obj.GetComponent<Material>();
            //material.SetData(Random.Range(0, materials.Count), Random.Range(0, meshes.Count));
        }
    }
}
