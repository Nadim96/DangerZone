using System;
using System.IO;
using System.Xml.Linq;
using Assets.Scripts.Player;
using Assets.Scripts.Settings;
using UnityEngine;
using AudioSettings = Assets.Scripts.Settings.AudioSettings;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// This script receives user input from the options screen. 
    /// Currently only used for volume settings. These settings are stored
    /// as XML-file when the user presses the save button.
    /// </summary>
    public class Options : MonoBehaviour
    {
        [SerializeField] private NumericUpDown _masterVolumeInput;
        [SerializeField] private NumericUpDown _gunVolumeInput;
        [SerializeField] private NumericUpDown _dialogueVolumeInput;
        [SerializeField] private Dropdown _dropdown;

        public static Options instance;
        private string _configPath;

        private void Awake()
        {
            instance = this;
            _configPath = Application.dataPath + "/StreamingAssets/Config/Settings.xml";
            LoadConfig();
        }

        private void OnEnable()
        {
            LoadConfig();
        }

        /// <summary>
        /// Loads XML config file.
        /// </summary>
        public void LoadConfig()
        {
            string path = _configPath;
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("Unable to load scenario. File not found.", path);

                XDocument xDocument = XDocument.Load(path);

                if (xDocument.Root == null)
                    throw new NullReferenceException("Unable to load config. Root of XML file is empty.");

                XElement sound = xDocument.Root.Element("Sound");

                if (sound == null)
                    throw new NullReferenceException("Unable to find Sound element in config.");

                foreach (XElement node in sound.Elements())
                {
                    float f;
                    if (!float.TryParse(node.Value, out f))
                        throw new Exception("Unable to parse value to float.");
                    SetVolume(node.Name.LocalName, f);
                }

                XElement xElement = xDocument.Root.Element("LaserType");
                if (xElement != null)
                {
                    string laser = xElement.Value;
                    if (!Enum.IsDefined(typeof(LaserType), laser))
                        throw new ArgumentOutOfRangeException(
                            string.Format("Unable to cast {0} to LaserType.", laser));

                    LaserType laserType = (LaserType) Enum.Parse(typeof(LaserType), laser);

                    _dropdown.Select((int) laserType);
                    MainSettings.Laser = laserType;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Unable to parse config file: " + e.Message + "\n Loading configuration defaults...");
                AudioSettings.LoadDefaults();
            }
        }


        /// <summary>
        /// Saves the config. Should be triggered when returning to the main menu from the settings menu.
        /// </summary>
        public void SaveConfig()
        {
            AudioSettings.MasterVolume = _masterVolumeInput.Value;
            AudioSettings.GunVolume = _gunVolumeInput.Value;
            AudioSettings.DialogueVolume = _dialogueVolumeInput.Value;

            if (!Enum.IsDefined(typeof(LaserType), _dropdown.Value))
                throw new ArgumentOutOfRangeException(
                    string.Format("Unable to cast {0} to LaserType.", _dropdown.Value));

            MainSettings.Laser = (LaserType) _dropdown.Value;

            XDocument xDocument = new XDocument
            (
                new XElement("Settings",
                    new XElement("Sound",
                        new XElement("MasterVolume", AudioSettings.MasterVolume),
                        new XElement("GunVolume", AudioSettings.GunVolume),
                        new XElement("DialogueVolume", AudioSettings.DialogueVolume)
                    ),
                    new XElement("LaserType", MainSettings.Laser.ToString())
                )
            );


            xDocument.Save(_configPath);
            Debug.Log("settings saved");
        }

        /// <summary>
        /// Stores the volume of a volume category. Also updates the UI if possible.
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        private void SetVolume(string nodeName, float value)
        {
            switch (nodeName)
            {
                case "MasterVolume":
                    AudioSettings.MasterVolume = value;
                    if (_masterVolumeInput != null)
                        _masterVolumeInput.Value = (int) value;
                    break;
                case "GunVolume":
                    AudioSettings.GunVolume = value;
                    if (_gunVolumeInput != null)
                        _gunVolumeInput.Value = (int) value;
                    break;
                case "DialogueVolume":
                    AudioSettings.DialogueVolume = value;
                    if (_dialogueVolumeInput != null)
                        _dialogueVolumeInput.Value = (int) value;
                    break;
                default:
                    throw new Exception("Unknown sound category. Unable to set volume.");
            }
        }
    }
}