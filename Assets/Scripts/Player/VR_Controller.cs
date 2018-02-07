using UnityEngine;
using Valve.VR;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Set the different button presses of a VR controller.
    /// </summary>
    public class VR_Controller : MonoBehaviour
    {
        protected EVRButtonId TouchpadButton = EVRButtonId.k_EButton_SteamVR_Touchpad;
        protected EVRButtonId GripButton = EVRButtonId.k_EButton_Grip;
        protected EVRButtonId TriggerButton = EVRButtonId.k_EButton_SteamVR_Trigger;

        // An instance to the controller
        protected SteamVR_Controller.Device Controller
        {
            get { return SteamVR_Controller.Input((int) TrackedObject.index); }
        }

        // An instance to the tracked object
        protected SteamVR_TrackedObject TrackedObject
        {
            get { return GetComponent<SteamVR_TrackedObject>(); }
        }

        protected virtual void Update()
        {
        }
    }
}