using UnityEngine;
using UnityEngine.Pool;

namespace StackProto
{
    public class ParticlePool : MonoBehaviour
    {
        [SerializeField] protected GameObject prefab = null;
        [SerializeField] protected bool collectionChecks = true;
        [SerializeField] protected int maxPoolSize = 10;
        
        protected ObjectPool<ParticleSystem> objectPool = null;

        private void Start()
        {
            objectPool = new ObjectPool<ParticleSystem>(
                OnCreatePoolObject, OnGetPoolObject, OnReturnedToPool, OnDestroyPoolObject);
        }
        
        private ParticleSystem OnCreatePoolObject()
        {
            var obj = Instantiate(prefab, transform.position, Quaternion.identity);
            obj.SetActive(false);
            var returnToPool = obj.AddComponent<ReturnToPool>();
            returnToPool.SetPool(objectPool);
            var system = obj.GetComponent<ParticleSystem>();
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            return system;
        }

        private void OnGetPoolObject(ParticleSystem system)
        {
            system.gameObject.SetActive(true);
            system.Play();
        }

        private void OnReturnedToPool(ParticleSystem system)
        {
            system.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(ParticleSystem system)
        {
            Destroy(system.gameObject);
        }
    }
}