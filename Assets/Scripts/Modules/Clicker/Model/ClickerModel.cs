using System;
using UniRx;
using UnityEngine;

namespace Modules.Clicker.Model
{
    public class ClickerModel : IDisposable
    {
        public ReactiveProperty<int> Currency { get; private set; } = new(0);
        public ReactiveProperty<int> Energy { get; private set; } = new(1000);
        public ReactiveProperty<bool> CanClick { get; private set; } = new(true);
        
        public Subject<Unit> OnCurrencyChanged { get; private set; } = new();
        public Subject<Unit> OnEnergyChanged { get; private set; } = new();
        public Subject<Unit> OnEnergyDepleted { get; private set; } = new();
        
        private readonly ClickerConfig _config;
        
        public ClickerModel(ClickerConfig config)
        {
            _config = config;
            InitializeDefaults();
        }
        
        private void InitializeDefaults()
        {
            Currency.Value = 0;
            Energy.Value = _config.InitialEnergy;
            UpdateCanClick();
        }
        
        public bool TrySpendEnergy(int amount)
        {
            if (Energy.Value >= amount)
            {
                Energy.Value -= amount;
                OnEnergyChanged.OnNext(Unit.Default);
                UpdateCanClick();
                return true;
            }
            
            OnEnergyDepleted.OnNext(Unit.Default);
            return false;
        }
        
        public void AddCurrency(int amount)
        {
            Currency.Value += amount;
            OnCurrencyChanged.OnNext(Unit.Default);
        }
        
        public void AddEnergy(int amount)
        {
            Energy.Value = Math.Min(Energy.Value + amount, _config.MaxEnergy);
            OnEnergyChanged.OnNext(Unit.Default);
            UpdateCanClick();
        }
        
        private void UpdateCanClick()
        {
            CanClick.Value = Energy.Value >= _config.ClickEnergyCost;
        }
        
        public void Dispose()
        {
            Currency?.Dispose();
            Energy?.Dispose();
            CanClick?.Dispose();
            OnCurrencyChanged?.Dispose();
            OnEnergyChanged?.Dispose();
            OnEnergyDepleted?.Dispose();
        }
    }
}