using UnityEngine;
using Zenject;
using Core.Navigation;
using Modules.Clicker.Model;
using Modules.Clicker.View;
using Modules.Clicker.Presenter;
using Modules.Weather.View;
using Modules.Dogs.View;

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
        }
        
        private void BindAsTabView<T>(T view) where T : MonoBehaviour, ITabView
        {
            Container.Bind<ITabView>()
                .FromInstance(view)
                .AsCached();
        }
    }
}