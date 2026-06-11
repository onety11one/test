using UnityEngine;
using Zenject;

namespace Modules.Clicker.VFX
{
    public class ClickParticlesPool : MonoMemoryPool<ParticleSystem>
    {
        [SerializeField] private ParticleSystem _prefab;
        
        protected override void OnCreated(ParticleSystem item)
        {
            item.Stop();
        }
        
        protected override void OnDespawned(ParticleSystem item)
        {
            item.Stop();
            item.gameObject.SetActive(false);
        }
        
        protected override void OnSpawned(ParticleSystem item)
        {
            item.gameObject.SetActive(true);
            item.Play();
        }
    }
}