using System;
using Assets.Scripts.Scenario;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This menu is shown when selecting the Random Scenario menu option.
    /// The class interfaces the scenario settings with the UI.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [SerializeField] private NumericUpDown _minEnemies;
        [SerializeField] private NumericUpDown _maxEnemies;
        [SerializeField] private NumericUpDown _minFriendlies;
        [SerializeField] private NumericUpDown _maxFriendlies;
        [SerializeField] private NumericUpDown _minRoomSize;
        [SerializeField] private NumericUpDown _maxRoomSize;
        [SerializeField] private Dropdown _targetType;
        [SerializeField] private Dropdown _weaponType;
        [SerializeField] private Dropdown _reactionSpeed;

        private void Start()
        {
            _minEnemies.Value = ScenarioSettings.MinEnemies;
            _maxEnemies.Value = ScenarioSettings.MaxEnemies;
            _minFriendlies.Value = ScenarioSettings.MinFriendlies;
            _maxFriendlies.Value = ScenarioSettings.MaxFriendlies;

            _minRoomSize.Value = ScenarioSettings.MinRoomSize;
            _maxRoomSize.Value = ScenarioSettings.MaxRoomSize;
            _targetType.Select((int) ScenarioSettings.TargetType);
            _weaponType.Select((int) ScenarioSettings.WeaponSize);

            _minEnemies.OnValueChanged += OnMinEnemiesChanged;
            _maxEnemies.OnValueChanged += OnMaxEnemiesChanged;
            _minFriendlies.OnValueChanged += OnMinFriendliesChanged;
            _maxFriendlies.OnValueChanged += OnMaxFriendliesChanged;

            _minRoomSize.OnValueChanged += MinRoomSizeOnOnValueChanged;
            _maxRoomSize.OnValueChanged += MaxRoomSizeOnOnValueChanged;

            _targetType.OnSelectedIndexChanged += OnTargetTypeChanged;
            _weaponType.OnSelectedIndexChanged += OnWeaponSizeChanged;
            _reactionSpeed.OnSelectedIndexChanged += _reactionSpeed_OnSelectedIndexChanged;
        }

        private void _reactionSpeed_OnSelectedIndexChanged(int obj)
        {
            switch (obj)
            {
                case 1:
                    ScenarioSettings.ReactionTime = 1;
                    break;
                case 2:
                    ScenarioSettings.ReactionTime = 0.5f;
                    break;
                case 3:
                    ScenarioSettings.ReactionTime = 0.3f;
                    break;
            }
        }

        private void LightsTypeOnOnSelectedIndexChanged(int i)
        {
            //i = 0 is the on selection
            ScenarioSettings.Lights = true;
        }

        private void MaxRoomSizeOnOnValueChanged(int i)
        {
            ScenarioSettings.MaxRoomSize = i;
        }

        private void MinRoomSizeOnOnValueChanged(int i)
        {
            ScenarioSettings.MinRoomSize = i;
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

            ScenarioSettings.LevelType = (LevelType) value;
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