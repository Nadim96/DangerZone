using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <inheritdoc />
    /// <summary>
    /// Item which communicates with a GunInterface in order to shoot.
    /// </summary>
    public class RangedWeapon : Weapon
    {
        public int Ammo { get; private set; }
        public int MagSize { get; set; }
        public float ReloadTime { get; set; }

        protected GunInterface GunInterface { get; set; }
         
        /// <inheritdoc />
        /// <summary>
        /// Equips the weapon and finds the GunInterface on the instantiated item GameObject.
        /// </summary>
        public override void Equip()
        { 
            base.Equip();

            Ammo = MagSize;

            GunInterface = Instance.GetComponent<GunInterface>();

            if (GunInterface == null)
            {
                throw new NullReferenceException("Unable to obtain a GunInterface script from instantiated object.");
            }
        }

        public void Reload()
        {
            Ammo = MagSize;
        }

        /// <inheritdoc />
        /// <summary>
        /// Shoot the weapon when target is known. It is up to the AI to wait before 
        /// firing the next shot to maintain a realistic rate of fire.
        /// </summary>
        public override void Use(GameObject target)
        {
            base.Use(target);

            if (target == Player.Player.Instance.PlayerCameraEye.gameObject)
            {
                GunInterface.ShootAtPlayer();
            }
            else
            {
                GunInterface.Shoot(target);
            }
            
            Ammo--;
        }
    }
}