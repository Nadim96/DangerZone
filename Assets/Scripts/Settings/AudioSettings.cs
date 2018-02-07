using Assets.Scripts.Audio;
using UnityEngine;

namespace Assets.Scripts.Settings
{
    /// <summary>
    /// Static class for storing and setting volume settings.
    /// </summary>
    public static class AudioSettings
    {
        private const float DEFAULT_MASTER_VOLUME = 0.5f;
        private const float DEFAULT_GUN_VOLUME = 0.5f;
        private const float DEFAULT_DIALOGUE_VOLUME = 0.5f;

        /// <summary>
        /// Master volume, overall volume level of the game. Must be a value between 0 and 10.
        /// </summary>
        public static float MasterVolume
        {
            get { return AudioListener.volume * 10; }
            set { AudioListener.volume = value / 10; }
        }

        /// <summary>
        /// Gun volume. Also includes explosions. Must be a value between 0 and 10.
        /// </summary>
        public static float GunVolume
        {
            get { return AudioController.GunVolume * 10; }
            set { AudioController.GunVolume = value / 10; }
        }

        /// <summary>
        /// Gun volume. Also includes explosions. Must be a value between 0 and 10.
        /// </summary>
        public static float DialogueVolume
        {
            get { return AudioController.DialogueVolume * 10; }
            set { AudioController.DialogueVolume = value / 10; }
        }

        /// <summary>
        /// In case of a corrupt config file, load default values.
        /// </summary>
        public static void LoadDefaults()
        {
            MasterVolume = DEFAULT_MASTER_VOLUME;
            GunVolume = DEFAULT_GUN_VOLUME;
            DialogueVolume = DEFAULT_DIALOGUE_VOLUME;
        }
    }
}