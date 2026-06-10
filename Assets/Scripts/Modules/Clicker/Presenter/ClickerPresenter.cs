using System;
using Cysharp.Threading.Tasks;
using UniRx;
using Modules.Clicker.Model;
using Modules.Clicker.View;

namespace Modules.Clicker.Presenter
{
    public class ClickerPresenter : IDisposable
    {
        private readonly ClickerModel _model;
        private readonly IClickerView _view;
        private readonly ClickerConfig _config;
        
        private readonly CompositeDisposable _disposables = new();
        private IDisposable _autoCollectSubscription;
        
        public ClickerPresenter(ClickerModel model, IClickerView view, ClickerConfig config)
        {
            _model = model;
            _view = view;
            _config = config;
        }
        
        public void Initialize()
        {
            BindView();
            BindModel();
            StartAutoCollect();
            StartEnergyRestore();
        }
        
        private void BindView()
        {
            _view.OnClickButton
                .Where(_ => _model.CanClick.Value)
                .Subscribe(_ => HandleClick())
                .AddTo(_disposables);
            
            _view.OnClickButton
                .Where(_ => !_model.CanClick.Value)
                .Subscribe(_ => _view.ShowEnergyDepletedEffect())
                .AddTo(_disposables);
        }
        
        private void BindModel()
        {
            _model.Currency
                .Subscribe(value => _view.SetCurrencyText($"{_config.CurrencyName}: {value}"))
                .AddTo(_disposables);
            
            _model.Energy
                .Subscribe(value => _view.SetEnergyText($"Energy: {value}/{_config.MaxEnergy}"))
                .AddTo(_disposables);
            
            _model.CanClick
                .Subscribe(canClick => UpdateButtonState(canClick))
                .AddTo(_disposables);
        }
        
        private void HandleClick()
        {
            if (!_model.TrySpendEnergy(_config.ClickEnergyCost)) return;
            
            _model.AddCurrency(_config.ClickReward);
            
            PlayVFX();
        }
        
        private void StartAutoCollect()
        {
            _autoCollectSubscription = Observable.Interval(TimeSpan.FromSeconds(_config.AutoCollectInterval))
                .Where(_ => _model.CanClick.Value)
                .Subscribe(_ => HandleAutoCollect())
                .AddTo(_disposables);
        }
        
        private void HandleAutoCollect()
        {
            if (!_model.TrySpendEnergy(_config.AutoCollectEnergyCost)) return;
            
            _model.AddCurrency(_config.AutoCollectReward);
            
            PlayVFX();
        }

        private void PlayVFX()
        {
            _view.PlayClickAnimation();
            _view.PlayClickSound();
            _view.PlayParticleEffect();
            
            if (_view.ParticleSpawnPoint != null && _view.CoinTarget != null)
            {
                _view.PlayCoinFlyAnimation(
                    _view.ParticleSpawnPoint.position, 
                    _view.CoinTarget.position
                );
            }
        }
        
        private void StartEnergyRestore()
        {
            Observable.Interval(TimeSpan.FromSeconds(_config.EnergyRestoreInterval))
                .Subscribe(_ => _model.AddEnergy(_config.EnergyRestoreAmount))
                .AddTo(_disposables);
        }
        
        private void UpdateButtonState(bool canClick)
        {
            var button = _view.ButtonTransform?.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.interactable = canClick;
            }
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            _autoCollectSubscription?.Dispose();
            _model?.Dispose();
        }
    }
}