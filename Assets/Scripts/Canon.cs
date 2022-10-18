using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StackProto
{
    public class Canon : MonoBehaviour
    {
        [SerializeField] private CanonData data;
        [SerializeField] private CanonQueue queue;
        [SerializeField] private Transform shotPosition;
        [SerializeField] private Transform shotDirection;
        
        public void Start()
        {
            if (data is null) return;

            StartCoroutine(ShootTimer());
        }

        IEnumerator ShootTimer()
        {
            while (true)
            {
                if (queue.Objects.TryDequeue(out var obj))
                {
                    Shoot(obj);
                }
                
                yield return new WaitForSeconds(data.shotDuration);
            }
        }

        void Shoot(GameObject obj)
        {
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                obj.SetActive(true);
                
                rb.position = shotPosition.position;
                rb.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                rb.velocity = Vector3.zero;
                
                var shotDir = Quaternion.AngleAxis(Random.Range(0, data.shotAngleRange), shotDirection.up) * Quaternion.AngleAxis(Random.Range(0, data.shotAngleRange), shotDirection.right) * shotDirection.forward;
                rb.AddForce(shotDir * (data.shotPower + Random.Range(0, data.shotPowerRange)), ForceMode.VelocityChange);
            }
        }
    }
}