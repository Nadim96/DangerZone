using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <inheritdoc />
    /// <summary>
    /// Weapon used for attacking at melee range.
    /// Currently not implemented.
    /// </summary>
    public class MeleeWeapon : Weapon
    {
        public override void Use(GameObject target)
        {
            // When in line of sight, attack, etc.
            // Owner.GetTarget();
            throw new NotImplementedException();
        }
    }
}