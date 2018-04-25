using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Sets the interaction from the vrcontroller set as a gun
    /// </summary>
    public class VR_Controller_Gun : VR_Controller
    {
        private const ushort HAPTIC_PULSE_DURATION = 3999;


        public  PlayerGunInterface PlayerGunInterface
        {
            get { return GetComponentInChildren<PlayerGunInterface>(); }
        }

        protected override void Update()
        {
            base.Update();
             
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayerGunInterface.Shoot();
            }

            if (PlayerGunInterface != null)
            {
                if (Controller.GetPressDown(GripButton))
                {
                  //  PlayerGunInterface.ReloadGun();
                }

                if (Controller.GetPressDown(TriggerButton))
                {
                    PlayerGunInterface.Shoot();
                    SteamVR_Controller.Input((int) Controller.index).TriggerHapticPulse(HAPTIC_PULSE_DURATION);
                }
            }
        }
    }
}