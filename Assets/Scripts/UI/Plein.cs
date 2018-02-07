using System;
using Assets.Scripts.Environment;
using Assets.Scripts.Scenario;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This menu is shown when selecting the Random Scenario menu option.
    /// The class interfaces the scenario settings with the UI.
    /// </summary>
    public class Plein : MonoBehaviour
    {
        [SerializeField] private NumericUpDown _minEnemies;
        [SerializeField] private NumericUpDown _maxEnemies;
        [SerializeField] private NumericUpDown _minFriendlies;
        [SerializeField] private NumericUpDown _maxFriendlies;

        [SerializeField] private Dropdown _targetType;
        [SerializeField] private Dropdown _weaponType;
        [SerializeField] private Dropdown _timeOfDayInput;
        [SerializeField] private Dropdown _weatherInput;

        private void Start()
        {
            _minEnemies.Value = ScenarioSettings.MinEnemies;
            _maxEnemies.Value = ScenarioSettings.MaxEnemies;
            _minFriendlies.Value = ScenarioSettings.MinFriendlies;
            _maxFriendlies.Value = ScenarioSettings.MaxFriendlies;
            _targetType.Select((int)ScenarioSettings.TargetType);
            _weaponType.Select((int)ScenarioSettings.WeaponSize);
            _timeOfDayInput.Select((int)ScenarioSettings.TimeOfDay);
            _weatherInput.Select((int)ScenarioSettings.Weather);

            _minEnemies.OnValueChanged += OnMinEnemiesChanged;
            _maxEnemies.OnValueChanged += OnMaxEnemiesChanged;
            _minFriendlies.OnValueChanged += OnMinFriendliesChanged;
            _maxFriendlies.OnValueChanged += OnMaxFriendliesChanged;
            _targetType.OnSelectedIndexChanged += OnTargetTypeChanged;
            _weaponType.OnSelectedIndexChanged += OnWeaponSizeChanged;
            _timeOfDayInput.OnSelectedIndexChanged += OnTimeOfDayChanged;
            _weatherInput.OnSelectedIndexChanged += OnWeatherChanged;
        }

        /// <summary>
        /// Sets the scenario TargetType.
        /// </summary>
        /// <param name="value"></param>
        private static void OnTargetTypeChanged(int value)
        {
            if (!Enum.IsDefined(typeof(TargetType), value))
                throw new ArgumentOutOfRangeException(
                    string.Format("Unable to cast {0} to TargetType.", value));

            ScenarioSettings.TargetType = (TargetType)value;
        }

        /// <summary>
        /// Sets the scenario WeaponSize.
        /// </summary>
        /// <param name="value"></param>
        private static void OnWeaponSizeChanged(int value)
        {
            if (!Enum.IsDefined(typeof(WeaponSize), value))
                throw new ArgumentOutOfRangeException(
                    string.Format("Unable to cast {0} to WeaponSize.", value));

            ScenarioSettings.WeaponSize = (WeaponSize)value;
        }


        /// <summary>
        /// Sets the scenario minimum amount of enemies.
        /// </summary>
        /// <param name="value"></param>
        private static void OnMinEnemiesChanged(int value)
        {
            ScenarioSettings.MinEnemies = value;
        }

        /// <summary>
        /// Sets the scenario maximum amount of enemies.
        /// </summary>
        /// <param name="value"></param>
        private static void OnMaxEnemiesChanged(int value)
        {
            ScenarioSettings.MaxEnemies = value;
        }

        /// <summary>
        /// Sets the scenario minimum amount of friendlies.
        /// </summary>
        /// <param name="value"></param>
        private static void OnMinFriendliesChanged(int value)
        {
            ScenarioSettings.MinFriendlies = value;
        }

        /// <summary>
        /// Sets the scenario maximum amount of friendlies.
        /// </summary>
        /// <param name="value"></param>
        private static void OnMaxFriendliesChanged(int value)
        {
            ScenarioSettings.MaxFriendlies = value;
        }
        /// <summary>
        /// Sets the time of day of the scenario.
        /// </summary>
        /// <param name="value">The value that changed</param>
        private static void OnTimeOfDayChanged(int value)
        {
            if (!Enum.IsDefined(typeof(TimeOfDayType), value))
                throw new ArgumentOutOfRangeException(
                    string.Format("Unable to cast {0} to TimeOfDayType.", value));

            ScenarioSettings.TimeOfDay = (TimeOfDayType)value;
        }

        /// <summary>
        /// Sets the weather of the scenario.
        /// </summary>
        /// <param name="value">The value that changed</param>
        private static void OnWeatherChanged(int value)
        {
            if (!Enum.IsDefined(typeof(WeatherType), value))
                throw new ArgumentOutOfRangeException(
                    string.Format("Unable to cast {0} to TimeOfDayType.", value));

            ScenarioSettings.Weather = (WeatherType)value;
        }
    }
}
