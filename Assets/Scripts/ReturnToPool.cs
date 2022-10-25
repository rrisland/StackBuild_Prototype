using UnityEngine;
using UnityEngine.Pool;

namespace StackProto
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ReturnToPool : MonoBehaviour
    {
        private IObjectPool<ParticleSystem> objectPool;
        private new ParticleSystem particleSystem;

        public void SetPool(ObjectPool<ParticleSystem> pool)
        {
            objectPool = pool;
        }

        public IObjectPool<ParticleSystem> GetPool()
        {
            return objectPool;
        }

        private void Awake()
        {
            if (!TryGetComponent(out particleSystem)) return;

            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
        
        private void OnParticleSystemStopped()
        {
            objectPool.Release(particleSystem);
        }
    }
}