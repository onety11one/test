using UnityEngine;
using Zenject;
using Modules.Clicker.Model;
using Modules.Clicker.View;
using Modules.Clicker.Presenter;

namespace Installers
{
    public class ClickerInstaller : MonoInstaller
    {
        [SerializeField] private ClickerView _clickerView;
        [SerializeField] private ClickerConfig _clickerConfig;
        
        public override void InstallBindings()
        {
            // Конфиг
            Container.Bind<ClickerConfig>()
                .FromScriptableObject(_clickerConfig)
                .AsSingle();
            
            // Model
            Container.Bind<ClickerModel>()
                .AsSingle()
                .NonLazy();
            
            // View
            Container.Bind<IClickerView>()
                .FromInstance(_clickerView)
                .AsSingle();
            
            // Presenter
            Container.Bind<ClickerPresenter>()
                .AsSingle()
                .OnInstantiated<ClickerPresenter>((ctx, presenter) =>
                {
                    presenter.Initialize();
                })
                .NonLazy();
        }
    }
}