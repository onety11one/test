using System;
using UniRx;
using UnityEngine;
using Core.Navigation;
using TMPro;

namespace Modules.Weather.View
{
    public class WeatherView : MonoBehaviour, ITabView, IWeatherView
    {
        [SerializeField] private GameObject _contentPanel;
        [SerializeField] private TMP_Text _weatherText;
        
        public GameObject GameObject => gameObject;
        
        private readonly Subject<Unit> _onTabSelected = new();
        private readonly Subject<Unit> _onTabDeselected = new();
        
        public IObservable<Unit> OnTabSelected => _onTabSelected;
        public IObservable<Unit> OnTabDeselected => _onTabDeselected;
        
        public void Show()
        {
            if (_contentPanel != null)
                _contentPanel.SetActive(true);
            else
                gameObject.SetActive(true);
            
            _onTabSelected.OnNext(Unit.Default);
        }
        
        public void Hide()
        {
            if (_contentPanel != null)
                _contentPanel.SetActive(false);
            else
                gameObject.SetActive(false);
            
            _onTabDeselected.OnNext(Unit.Default);
        }
        
        public void SetWeather(string temperature)
        {
            if (_weatherText != null)
            {
                _weatherText.text = $"Today - {temperature}";
            }
        }
    }
}