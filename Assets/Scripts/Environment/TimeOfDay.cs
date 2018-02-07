using System;
using System.Collections.Generic;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// Changes lighting and skybox of the scene to match the selected time of day.
    /// </summary>
    public class TimeOfDay : MonoBehaviour
    {
        [SerializeField] private Light _sun;
        [SerializeField] private List<Light> _streetLights;
        [SerializeField] private Material _noonSky;
        [SerializeField] private Material _afternoonSky;
        [SerializeField] private Material _eveningSky;

        private void Start()
        {
            if (_sun == null)
                throw new NullReferenceException(
                    "TimeOfDay has no reference to the Sun (directional light).");

            if (_noonSky == null || _afternoonSky == null || _eveningSky == null)
                throw new NullReferenceException("Missing sky materials in TimeOfDay.");

            SetTimeOfDay(ScenarioSettings.TimeOfDay);
        }

        /// <summary>
        /// Turns the street lights on or off.
        /// </summary>
        /// <param name="emitLight"></param>
        private void ToggleStreetLights(bool emitLight)
        {
            foreach (Light streetLight in _streetLights)
            {
                streetLight.enabled = emitLight;
            }
        }

        /// <summary>
        /// Sets the time of day by changing the sky, 
        /// the angle and intensity of the sun (directional light)
        /// and by turning on/off the street lights.
        /// </summary>
        public void SetTimeOfDay(TimeOfDayType timeOfDay)
        {
            switch (timeOfDay)
            {
                case TimeOfDayType.Noon:
                    ConfigureScene(_noonSky, new Vector3(70, -170, 0), 1.35f, false);
                    break;

                case TimeOfDayType.Afternoon:
                    ConfigureScene(_afternoonSky, new Vector3(35, -177.5f, 0), 1.1f, false);
                    break;

                case TimeOfDayType.Evening:
                    ConfigureScene(_eveningSky, new Vector3(15, 177.5f, 0), 0.8f, true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("timeOfDay", timeOfDay,
                        "Tried setting time of day to unsupported value.");
            }
        }

        /// <summary>
        /// Changes the looks of the scene with the specified parameters.
        /// </summary>
        /// <param name="skybox">The skybox material to display in the scene</param>
        /// <param name="sunAngle">The angle of the sun (directional light)</param>
        /// <param name="sunIntensity">The intensity of the sun (directional light)</param>
        /// <param name="streetLightsOn">Should the street lights be on?</param>
        private void ConfigureScene(Material skybox, Vector3 sunAngle, float sunIntensity, bool streetLightsOn)
        {
            if (skybox == null)
                throw new ArgumentNullException("skybox");

            if (sunIntensity < 0)
                throw new ArgumentOutOfRangeException("sunIntensity");

            RenderSettings.skybox = skybox;
            _sun.transform.eulerAngles = sunAngle;
            _sun.intensity = sunIntensity;
            ToggleStreetLights(streetLightsOn);
        }
    }
}