using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StackProto;
using UnityEngine;

namespace StackProto
{
    public class BuildingSpace : MonoBehaviour
    {
        [SerializeField] private BuildingData data;

        private Vector3 origin;
        private float lastDownHeight = 0;
        private float height = 0;

        private void Start()
        {
            origin = transform.position;
        }
        
        public void Build(UnityEngine.Material material)
        {
            height += data.floorHeight;
            
            var obj = Instantiate(data.floorTemplate, transform);
            obj.transform.position += Vector3.up * height;

            if (obj.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.sharedMaterial = material;
            }

            if (!data.isFloorDown) return;

            if (height - lastDownHeight > data.floorHeightMax)
            {
                transform.DOMove(origin + Vector3.down * (height - data.floorHeightMax), 2.0f).SetEase(Ease.OutQuad);
                //transform.position = origin + Vector3.down * (height - data.floorHeightMax);
                lastDownHeight = height;
            }

            Debug.Log("Build(" + name +"): " + material.name + " Material");
        }
    }
}
