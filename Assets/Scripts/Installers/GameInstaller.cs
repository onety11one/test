using UnityEngine;
using Zenject;
using Core.Navigation;
using Core.Network;
using Modules.Clicker.Model;
using Modules.Clicker.View;
using Modules.Clicker.Presenter;
using Modules.Clicker.VFX;
using Modules.Weather.View;
using Modules.Dogs.View;
using Modules.Weather;
using Modules.Weather.Model;
using Modules.Weather.Presenter;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private ClickerConfig _clickerConfig;
        
        [Header("Tab Views")]
        [SerializeField] private ClickerView _clickerView;
        [SerializeField] private WeatherView _weatherView;
        [SerializeField] private DogsView _dogsView;
        
        [Header("Navigation")]
        [SerializeField] private TabButton[] _tabButtons;
        
        [Header("VFX Prefabs")]
        [SerializeField] private ParticleSystem _clickParticlesPrefab;
        [SerializeField] private RectTransform _flyingCoinPrefab;
        [SerializeField] private Transform _vfxParent;
        
        public override void InstallBindings()
        {
            Container.Bind<ClickerConfig>()
                .FromScriptableObject(_clickerConfig)
                .AsSingle();
            
            BindAsTabView(_clickerView);
            BindAsTabView(_weatherView);
            BindAsTabView(_dogsView);
            
            Container.Bind<IClickerView>()
                .FromInstance(_clickerView)
                .AsSingle();
            
            Container.Bind<ClickerModel>()
                .AsSingle()
                .NonLazy();
                
            Container.Bind<ClickerPresenter>()
                .AsSingle()
                .OnInstantiated<ClickerPresenter>((ctx, presenter) =>
                {
                    presenter.Initialize();
                })
                .NonLazy();
            
            Container.Bind<NavigationController>()
                .AsSingle()
                .OnInstantiated<NavigationController>((ctx, controller) =>
                {
                    var tabs = ctx.Container.ResolveAll<ITabView>();
                    
                    for (int i = 0; i < tabs.Count && i < _tabButtons.Length; i++)
                    {
                        controller.AddTab(tabs[i], _tabButtons[i]);
                    }
                    
                    controller.SwitchTab(0);
                })
                .NonLazy();
            
            var vfxContainer = new GameObject("VFXPoolContainer");
            vfxContainer.transform.SetParent(_vfxParent);
            
            Container.BindMemoryPool<ParticleSystem, ClickParticlesPool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(_clickParticlesPrefab)
                .UnderTransform(vfxContainer.transform)
                .AsCached();

            Container.BindMemoryPool<RectTransform, FlyingCoinPoolImpl>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(_flyingCoinPrefab)
                .UnderTransform(vfxContainer.transform)
                .AsCached();
            
            Container.Bind<ClickerVFXManager>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IWeatherView>()
                .FromInstance(_weatherView)
                .AsSingle();
            
            Container.Bind<WeatherModel>()
                    .AsSingle()
                    .NonLazy();
                
            Container.Bind<WeatherPresenter>()
                    .AsSingle()
                    .OnInstantiated<WeatherPresenter>((ctx, presenter) =>
                    {
                        presenter.Initialize();
                    })
                    .NonLazy();
            
            Container.Bind<WeatherService>()
                .AsSingle();
            
            Container.Bind<NetworkRequestQueue>()
                    .AsSingle()
                    .NonLazy();
        }
        
        private void BindAsTabView<T>(T view) where T : MonoBehaviour, ITabView
        {
            Container.Bind<ITabView>()
                .FromInstance(view)
                .AsCached();
        }
    }
}