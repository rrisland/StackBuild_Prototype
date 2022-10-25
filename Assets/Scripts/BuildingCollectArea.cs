using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace StackProto
{
    public class BuildingCollectArea : NetworkBehaviour
    {
        [SerializeField] private BuildingSpace buildingSpace;
        [SerializeField] private BuildingData buildingData;
        [SerializeField] private MaterialData materialData;

        private List<float> materialCounter = new List<float>();

        [ClientRpc]
        private void StakingClientRPC()
        {
            
        }

        private void Start()
        {
            // if (!NetworkManager.Singleton.IsServer)
            //     return;
            
            for (int i = 0; i < materialData.materials.Count; i++)
            {
                materialCounter.Add(0);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // if (!NetworkManager.Singleton.IsServer)
            //     return;
            
            if (buildingData is null || materialData is null) return;

            if (other.gameObject.TryGetComponent(out StackProto.Material material))
                StackingOfParts(material.MaterialIndex);

            Destroy(other.gameObject);
        }

        void StackingOfParts(int materialIndex)
        {
            materialCounter[materialIndex]++;

            var list = buildingData.list.FindAll(
                d => d.beforeMaterial.name.Equals(materialData.materials[materialIndex].name));

            foreach (var data in list)
            {
                if (data.requiredAmount <= materialCounter[materialIndex])
                {
                    buildingSpace.Build(data.afterMaterial);
                    materialCounter[materialIndex] -= data.requiredAmount;
                }
            }
        }
    }
}
