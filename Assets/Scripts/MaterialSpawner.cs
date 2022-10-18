using System.Collections;
using System.Collections.Generic;
using StackProto;
using UnityEngine;
using UnityEngine.SocialPlatforms;


namespace StackProto
{
    public class MaterialSpawner : MonoBehaviour
    {
        [SerializeField] private MaterialData materialData;
        [SerializeField] private MaterialSpawnerData spawnerData;
        [SerializeField] private List<CanonQueue> queues;

        private List<UnityEngine.Material> materials => materialData.materials;
        private List<UnityEngine.Mesh> meshes => materialData.meshes;
    
        void Start()
        {
            if (materialData is null || queues is null) return;
            
            StackProto.Material.Initialize(materialData);
            StartCoroutine(SpawnTimer());
        }
    
        IEnumerator SpawnTimer()
        {
            while (true)
            {
                int index = Random.Range(0, queues.Count);
                
                for (int i = 0; i < spawnerData.quantity; i++)
                {
                    Spawn(out var obj);
                    queues[index].Objects.Enqueue(obj);
                }
                
                yield return new WaitForSeconds(spawnerData.duration);
            }
        }

        void Spawn(out GameObject obj)
        {
            obj = new GameObject("Material");
            obj.SetActive(false);
            obj.transform.position = Vector3.down * 100;
            obj.transform.SetParent(transform);

            Material material = obj.AddComponent<Material>();
            material.SetData(Random.Range(0, materials.Count), Random.Range(0, meshes.Count));
        }
    }
}
