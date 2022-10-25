using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StackProto;
using UnityEngine;

namespace StackProto
{
    public class BuildingSpace : MonoBehaviour
    {
        private struct StackQueueEntry
        {
            public Transform building;
            public float y;
        }

        [SerializeField] private BuildingData data;

        private Vector3 origin;
        private float lastDownHeight = 0;
        private float height = 0;
        private bool isStackAnimating = false;
        private readonly Queue<StackQueueEntry> stackQueue = new ();

        public float Height => height;

        private void Start()
        {
            origin = transform.position;
        }
        
        public void Build(UnityEngine.Material material)
        {
            height += data.floorHeight;
            
            var obj = Instantiate(data.floorTemplate, transform);
            AnimateStack(obj.transform, height);

            if (obj.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.sharedMaterial = material;
            }
        }

        private void AnimateStack(Transform building, float y)
        {
            stackQueue.Enqueue(new StackQueueEntry
            {
                building = building,
                y = y,
            });

            if (isStackAnimating) return;
            isStackAnimating = true;
            StartCoroutine(StackCoroutine());
        }

        private IEnumerator StackCoroutine()
        {
            while (stackQueue.TryDequeue(out var entry))
            {
                entry.building.transform.DOLocalMoveY(entry.y, 0.8f).From(entry.y + 20).SetEase(Ease.InCubic);
                var baseY = entry.y - data.floorHeightMax;
                if (data.isFloorDown && baseY - lastDownHeight > 0)
                {
                    transform.DOLocalMoveY(-baseY, 1.0f).SetEase(Ease.OutBack).SetDelay(0.8f);
                    lastDownHeight = baseY;
                }
                yield return new WaitForSeconds(0.07f);
            }

            isStackAnimating = false;
        }
    }
}
