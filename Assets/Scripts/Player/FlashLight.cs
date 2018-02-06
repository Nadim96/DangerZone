using UnityEngine;

namespace Assets.Scripts.Player
{
    public class FlashLight : VR_Controller
    {
        public GameObject Light;
        protected override void Update()
        {
            //just make all buttons turn on light to prevent arguing about the best button
            if (Controller.GetPressDown(GripButton) ||
                Controller.GetPressDown(TouchpadButton) ||
                Controller.GetPressDown(TriggerButton))
            {
                Light.SetActive(!Light.activeInHierarchy);
            }
        }
    }
}