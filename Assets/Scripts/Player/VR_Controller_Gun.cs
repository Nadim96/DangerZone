using Assets.Scripts.Items;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Sets the interaction from the vrcontroller set as a gun
    /// </summary>
    public class VR_Controller_Gun : VR_Controller
    {
        private const ushort HAPTIC_PULSE_DURATION = 3999;

        private PlayerGunInterface PlayerGunInterface
        {
            get { return GetComponentInChildren<PlayerGunInterface>(); }
        }

        protected override void Update()
        {
            base.Update();

            if (PlayerGunInterface != null)
            {
                if (Controller.GetPressDown(GripButton))
                {
                    PlayerGunInterface.ReloadGun();
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