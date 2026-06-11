using UnityEngine;
using Zenject;

namespace Modules.Clicker.VFX
{
    public class FlyingCoinPoolImpl : MonoMemoryPool<RectTransform>
    {
        [SerializeField] private RectTransform _prefab;
        
        protected override void OnCreated(RectTransform item)
        {
            item.gameObject.SetActive(false);
        }
        
        protected override void OnDespawned(RectTransform item)
        {
            item.gameObject.SetActive(false);
        }
        
        protected override void OnSpawned(RectTransform item)
        {
            item.gameObject.SetActive(true);
        }
    }
}