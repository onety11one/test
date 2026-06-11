using System;
using UniRx;

namespace Modules.Weather.View
{
    public interface IWeatherView
    {
        void SetWeather(string temperature);
        IObservable<Unit> OnTabSelected { get; }
        IObservable<Unit> OnTabDeselected { get; }
    }
}