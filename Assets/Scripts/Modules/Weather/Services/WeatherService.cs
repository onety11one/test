using System;
using Modules.Weather.Model;
using UnityEngine;
using UnityEngine.Networking;

namespace Modules.Weather.Services
{
    public class WeatherService
    {
        private const string API_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
        
        public UnityWebRequest CreateWeatherRequest()
        {
            var request = UnityWebRequest.Get(API_URL);
            request.SetRequestHeader("User-Agent", "UnityGame/1.0");
            request.SetRequestHeader("Accept", "application/json");
            return request;
        }
        
        public WeatherData ParseWeatherResponse(string json)
        {
            try
            {
                var response = JsonUtility.FromJson<WeatherResponse>(json);
                var period = response.properties.periods[0];
                
                return new WeatherData
                {
                    Temperature = $"{period.temperature}F",
                };
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to parse weather: {e.Message}");
                return null;
            }
        }
    }
    
    [Serializable]
    public class WeatherResponse
    {
        public Properties properties;
    }
    
    [Serializable]
    public class Properties
    {
        public Period[] periods;
    }
    
    [Serializable]
    public class Period
    {
        public int temperature;
        public string shortForecast;
    }
}