using System;
using Assets.Scripts.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// This MonoBehaviour must be attached to a Controller in the camera rig. 
    /// It simulates normal UI clicking behaviour while using the laser pointer on the controller.
    /// </summary>
    [RequireComponent(typeof(SteamVR_LaserPointer))]
    public class LaserPointer : MonoBehaviour
    {
        private SteamVR_LaserPointer _laserPointer;
        private SteamVR_TrackedController _trackedController;

        private LaserType current;
        private bool _started;
        public const float LaserThickness = 0.004f;

        private void OnEnable()
        {
            _laserPointer = GetComponent<SteamVR_LaserPointer>();

            if (_laserPointer == null)
                throw new NullReferenceException();

            _laserPointer.PointerIn += HandlePointerIn;
            _laserPointer.PointerOut += HandlePointerOut;

            _trackedController = GetComponent<SteamVR_TrackedController>() ??
                                 GetComponentInParent<SteamVR_TrackedController>();

            if (_trackedController == null)
                throw new NullReferenceException();

            _trackedController.TriggerClicked += HandleTriggerClicked;
        }

        private void OnDisable()
        {
            _laserPointer.PointerIn -= HandlePointerIn;
            _laserPointer.PointerOut -= HandlePointerOut;

            _trackedController.TriggerClicked -= HandleTriggerClicked;
        }

        private void OnDestroy()
        {
            _laserPointer.PointerIn -= HandlePointerIn;
            _laserPointer.PointerOut -= HandlePointerOut;

            _trackedController.TriggerClicked -= HandleTriggerClicked;
        }

        /// <summary>
        /// Clicked on something. Click on currently selected UI element if there is one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HandleTriggerClicked(object sender, ClickedEventArgs e)
        {
            EventSystem current = EventSystem.current;
            if (current == null || current.currentSelectedGameObject == null) return;

            ExecuteEvents.Execute(
                current.currentSelectedGameObject,
                new PointerEventData(current),
                ExecuteEvents.submitHandler);
        }

        void Start()
        {
            //make sure laser is always on in menu for easier menu navigating
            _laserPointer.thickness = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu" ? LaserThickness : GetLaserThickness(MainSettings.Laser);
        }

        private float GetLaserThickness(LaserType laserType)
        {
            switch (laserType)
            {
                case LaserType.Always:
                  return LaserThickness;
                case LaserType.Never:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Is hovering over something. Check for buttons and select if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HandlePointerIn(object sender, PointerEventArgs e)
        {
            var button = e.target.GetComponent<Button>();

            if (button != null)
            {
                button.Select();
            }
        }

        /// <summary>
        /// Stopped hovering over something. Check for buttons and deselect if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HandlePointerOut(object sender, PointerEventArgs e)
        {
            var button = e.target.GetComponent<Button>();
            EventSystem current = EventSystem.current;
            if (button != null && current != null)
            {
                current.SetSelectedGameObject(null);
            }
        }
    }
}