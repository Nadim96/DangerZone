using System;
using Assets.Scripts.NPCs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Items
{
    /// <inheritdoc />
    /// <summary>
    /// The NPC can use an item by having an Item instance. This class builds ontop of the actual items and provides functionality for animations.
    /// </summary>
    public class Item : IDisposable
    {
        public NPC Owner { get; set; }
        public GameObject Model { get; set; }
        public GameObject Instance { get; set; }
        public string EquipAnimation { get; set; }
        public string UnequipAnimation { get; set; }
        public string UseAnimation { get; set; }
        public string IdleAnimation { get; set; }
        public bool IsEquipped { get; protected set; }

        public virtual void Dispose()
        {
            Object.Destroy(Instance);
        }
         
        /// <summary>
        /// Equips the item by creating an instance and placing it in the equiphand. Disables the NavMeshAgent to allow item animations.
        /// </summary>
        public virtual void Equip()
        {
            if (IsEquipped) return;

            IsEquipped = true;

            Instance = Object.Instantiate(Model, Owner.EquipHand.transform);

            Owner.Animator.SetTrigger(EquipAnimation);
        }

        /// <summary>
        /// Unequips the item by triggering the unequip animation and destroying the item instance.
        /// </summary>
        public virtual void Unequip()
        {
            if (!IsEquipped) return;

            Owner.Animator.SetTrigger(UnequipAnimation);

            Object.Destroy(Instance);

            IsEquipped = false;
        }

        /// <summary>
        /// Not mandatory: use the item (for example: shoot)
        /// </summary>
        public virtual void Use(GameObject target)
        {
            Owner.Animator.SetTrigger(UseAnimation);
        }
    }
}