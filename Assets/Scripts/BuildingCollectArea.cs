using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace StackProto
{
    public class BuildingCollectArea : MonoBehaviour
    {
        [SerializeField] private BuildingSpace buildingSpace;
        [SerializeField] private BuildingData buildingData;
        [SerializeField] private MaterialData materialData;

        private List<float> materialCounter = new List<float>();

        private void Start()
        {
            for (int i = 0; i < materialData.materials.Count; i++)
            {
                materialCounter.Add(0);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (buildingData is null || materialData is null) return;

            if (other.gameObject.TryGetComponent(out StackProto.Material material))
            {
                materialCounter[material.MaterialIndex]++;

                var list = buildingData.list.FindAll(
                    d => d.beforeMaterial.name.Equals(materialData.materials[material.MaterialIndex].name));

                foreach (var data in list)
                {
                    if (data.requiredAmount <= materialCounter[material.MaterialIndex])
                    {
                        buildingSpace.Build(data.afterMaterial);
                        materialCounter[material.MaterialIndex] -= data.requiredAmount;
                    }
                }
            }

            Destroy(other.gameObject);
        }
    }
}
