using System;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// Sets the weather in the scene. Currently only enables/disables a rain particle effect.
    /// </summary>
    public class Weather : MonoBehaviour
    {
        [SerializeField] private GameObject _rainEffect;

        private void Start()
        {
            if (_rainEffect == null)
                throw new NullReferenceException("Weather has no reference to a RainEffect GameObject.");

            SetWeather(ScenarioSettings.Weather);
        }

        /// <summary>
        /// Sets the type of weather by enabling/disabling the rain effect.
        /// </summary>
        /// <param name="weather"></param>
        private void SetWeather(WeatherType weather)
        {
            _rainEffect.SetActive(weather == WeatherType.Rainy);
        }
    }
}