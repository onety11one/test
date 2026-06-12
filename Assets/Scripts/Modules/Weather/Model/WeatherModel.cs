using System;
using UniRx;

namespace Modules.Weather.Model
{
    [Serializable]
    public class WeatherData
    {
        public string Temperature;
        public string IconUrl;
    }
    
    public class WeatherModel : IDisposable
    {
        public ReactiveProperty<WeatherData> CurrentWeather { get; } = new();
        
        public void Dispose()
        {
            CurrentWeather?.Dispose();
        }
    }
}