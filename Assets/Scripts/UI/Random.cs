using System;
using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Scenario;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This menu is shown when selecting the Random Scenario menu option.
    /// The class interfaces the scenario settings with the UI.
    /// </summary>
    public class Random : MonoBehaviour
    {
        [SerializeField] private NumericUpDown _minEnemies;
        [SerializeField] private NumericUpDown _maxEnemies;
        [SerializeField] private NumericUpDown _minFriendlies;
        [SerializeField] private NumericUpDown _maxFriendlies;

        [SerializeField] private Dropdown _weaponType;
        [SerializeField] private Dropdown _MovementType;

        private void Start()
        {
            _minEnemies.Value = ScenarioSettings.MinEnemies;
            _maxEnemies.Value = ScenarioSettings.MaxEnemies;
            _minFriendlies.Value = ScenarioSettings.MinFriendlies;
            _maxFriendlies.Value = ScenarioSettings.MaxFriendlies;
            _weaponType.Select((int)ScenarioSettings.WeaponSize);

            _minEnemies.OnValueChanged += OnMinEnemiesChanged;
            _maxEnemies.OnValueChanged += OnMaxEnemiesChanged;
            _minFriendlies.OnValueChanged += OnMinFriendliesChanged;
            _maxFriendlies.OnValueChanged += OnMaxFriendliesChanged;
            _weaponType.OnSelectedIndexChanged += OnWeaponSizeChanged;

            _MovementType.OnSelectedIndexChanged += _MovementType_OnSelectedIndexChanged;
        }

        private void _MovementType_OnSelectedIndexChanged(int obj)
        {
            ScenarioSettings.MovementType = (MovementType)obj;
        }

        private void LightsOnOnSelectedIndexChanged(int i)
        {
            //i = 0 is the on selection
            ScenarioSettings.Lights = (i == 0);
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

            ScenarioSettings.TargetType = (TargetType) value;
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

            ScenarioSettings.WeaponSize = (WeaponSize) value;
        }

        /// <summary>
        /// Sets the scenario LevelType.
        /// </summary>
        /// <param name="value"></param>
        private static void OnLevelTypeChanged(int value)
        {
            if (!Enum.IsDefined(typeof(LevelType), value))
                throw new ArgumentOutOfRangeException(
                    string.Format("Unable to cast {0} to LevelType.", value));

            ScenarioSettings.LevelType = (LevelType)value;
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
    }
}