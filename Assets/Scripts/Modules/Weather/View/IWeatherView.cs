using System;
using UniRx;
using UnityEngine;

namespace Modules.Weather.View
{
    public interface IWeatherView
    {
        void SetWeather(string temperature);
        void SetWeatherIcon(Sprite icon);
        IObservable<Unit> OnTabSelected { get; }
        IObservable<Unit> OnTabDeselected { get; }
    }
}