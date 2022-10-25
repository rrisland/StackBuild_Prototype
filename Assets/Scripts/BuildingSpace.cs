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
        [SerializeField] private Vector2 spaceSize;
        [SerializeField] private int rows = 1;
        [SerializeField] private int columns = 1;

        private Vector3 origin;
        private float lastDownHeight = 0;
        private float height = 0;
        private int buildingsInCurrentFloor;
        private bool isStackAnimating = false;
        private readonly Queue<StackQueueEntry> stackQueue = new ();

        public float Height => height;

        private void Start()
        {
            origin = transform.position;
        }
        
        public void Build(UnityEngine.Material material)
        {
            if (buildingsInCurrentFloor == rows * columns)
            {
                height += data.floorHeight;
                buildingsInCurrentFloor = 0;
            }

            var topleft = transform.position - new Vector3(spaceSize.x, 0, spaceSize.y) / 2;
            int row = rows - 1 - buildingsInCurrentFloor / columns;
            int col = buildingsInCurrentFloor % columns;
            var obj = Instantiate(data.floorTemplate, topleft + new Vector3((col + 0.5f) / columns * spaceSize.x, 0, (row + 0.5f) / rows * spaceSize.y), Quaternion.identity, transform);
            obj.transform.localScale = new Vector3(1.0f / columns, 1, 1.0f / rows);
            AnimateStack(obj.transform, height + data.floorHeight / 2);
            buildingsInCurrentFloor++;

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
                yield return new WaitForSeconds(0.05f);
            }

            isStackAnimating = false;
        }
    }
}
