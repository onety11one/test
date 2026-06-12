using System;
using UniRx;
using UnityEngine;
using Core.Network;
using Modules.Weather.Model;
using Modules.Weather.Services;
using Modules.Weather.View;

namespace Modules.Weather.Presenter
{
    public class WeatherPresenter : IDisposable
    {
        private readonly WeatherModel _model;
        private readonly IWeatherView _view;
        private readonly WeatherService _weatherService;
        private readonly NetworkRequestQueue _requestQueue;
        private readonly CompositeDisposable _disposables = new();
        
        private NetworkRequestQueue.QueuedRequest _currentWeatherRequest;
        private IDisposable _weatherUpdateSubscription;
        private bool _isTabActive;
        
        public WeatherPresenter(WeatherModel model, IWeatherView view, WeatherService weatherService, NetworkRequestQueue requestQueue)
        {
            _model = model;
            _view = view;
            _weatherService = weatherService;
            _requestQueue = requestQueue;
        }
        
        public void Initialize()
        {
            _model.CurrentWeather
                .Subscribe(weather =>
                {
                    if (weather != null)
                    {
                        _view.SetWeather(weather.Temperature);
                    }
                })
                .AddTo(_disposables);
            
            _view.OnTabSelected
                .Subscribe(_ =>
                {
                    Debug.Log("Weather: starting requests");
                    _isTabActive = true;
                    StartWeatherUpdates();
                })
                .AddTo(_disposables);
            
            _view.OnTabDeselected
                .Subscribe(_ =>
                {
                    Debug.Log("Weather: removing requests");
                    _isTabActive = false;
                    StopWeatherUpdates();
                })
                .AddTo(_disposables);
        }
        
        private void StartWeatherUpdates()
        {
            StopWeatherUpdates();
            
            RequestWeather();
            
            _weatherUpdateSubscription = Observable.Interval(TimeSpan.FromSeconds(5))
                .Where(_ => _isTabActive)
                .Subscribe(_ => RequestWeather())
                .AddTo(_disposables);
        }
        
        private void StopWeatherUpdates()
        {
            _weatherUpdateSubscription?.Dispose();
            
            if (_currentWeatherRequest != null)
            {
                Debug.Log("Weather: cancelling request");
                _requestQueue.CancelRequest(_currentWeatherRequest);
                _currentWeatherRequest = null;
            }
        }
        
        private void RequestWeather()
        {
            if (!_isTabActive) return;
            
            Debug.Log("Weather: adding request to queue");
            
            _currentWeatherRequest = _requestQueue.Enqueue(
                requestFactory: () => _weatherService.CreateWeatherRequest(),
                onSuccess: (json) =>
                {
                    Debug.Log("Weather: request completed");
                    var weatherData = _weatherService.ParseWeatherResponse(json);
                    if (weatherData != null)
                    {
                        _model.CurrentWeather.Value = weatherData;
                    }
                },
                onError: (error) =>
                {
                    Debug.Log($"Weather: request failed: {error}");
                }
            );
        }
        
        public void Dispose()
        {
            StopWeatherUpdates();
            _disposables?.Dispose();
            _model?.Dispose();
        }
    }
}