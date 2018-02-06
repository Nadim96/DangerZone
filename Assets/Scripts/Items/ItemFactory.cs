using System;
using Assets.Scripts.NPCs;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Items
{
    /// <inheritdoc />
    /// <summary>
    /// Creates instances of all known weapons, which can then be placed in the hands of NPC's.
    /// </summary>
    public class ItemFactory : MonoBehaviour
    {
        private const string ANIM_IDLE = "Idle";

        private const string ANIM_EQUIP_PHONE = "Equip Phone";
        private const string ANIM_EQUIP_PISTOL = "Equip Pistol";
        private const string ANIM_EQUIP_RIFLE = "Equip Rifle";

        private const string ANIM_HOLD_WEAPON = "Hold Weapon";
        private const string ANIM_ATTACK_WEAPON = "Attack Weapon";
        private const string ANIM_DROP_WEAPON = "Drop Weapon";

        public ObjectPool CasingPool;
        // P99
        [SerializeField] private GameObject _modelP99;
        [SerializeField] private float _reloadTimeP99;
        [SerializeField] private float _rangeP99;
        [SerializeField] private int _magSizeP99;

        // AK
        [SerializeField] private GameObject _modelAK;
        [SerializeField] private float _reloadTimeAK;
        [SerializeField] private float _rangeAK;
        [SerializeField] private int _magSizeAK;

        // Phone
        [SerializeField] private GameObject _modelSmartphone;

        public static ItemFactory Instance { get; private set; }

        private void Start()
        {
            Instance = this;

            _modelP99.GetComponent<GunInterface>().casingPool = CasingPool;
            _modelAK.GetComponent<GunInterface>().casingPool = CasingPool;
        }

        /// <summary>
        /// Returns a predefined item of the specified ItemType.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Item CreateItem(ItemType type, NPC owner)
        {
            switch (type)
            {
                case ItemType.P99:
                    return new RangedWeapon
                    {
                        Owner = owner,
                        Model = Instance._modelP99,
                        EquipAnimation = ANIM_EQUIP_PISTOL,
                        UseAnimation = ANIM_ATTACK_WEAPON,
                        IdleAnimation = ANIM_HOLD_WEAPON,
                        UnequipAnimation = ANIM_DROP_WEAPON,
                        ReloadTime = _reloadTimeP99,
                        Range = _rangeP99,
                        MagSize = _magSizeP99,
                    };
                case ItemType.Ak:
                    return new RangedWeapon
                    {
                        Owner = owner,
                        Model = Instance._modelAK,
                        EquipAnimation = ANIM_EQUIP_RIFLE,
                        UseAnimation = ANIM_ATTACK_WEAPON,
                        IdleAnimation = ANIM_HOLD_WEAPON,
                        UnequipAnimation = ANIM_DROP_WEAPON,
                        ReloadTime = _rangeAK,
                        Range = _rangeAK,
                        MagSize = _magSizeAK
                    };
                case ItemType.Phone:
                    return new Item
                    {
                        Owner = owner,
                        Model = _modelSmartphone,
                        EquipAnimation = ANIM_EQUIP_PHONE,
                        UnequipAnimation = ANIM_IDLE,
                    };
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        /// <summary>
        /// Currently returns a P99, since that is the only weapon in the game.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Item GetRandomWeapon(NPC owner)
        {
            return CreateItem(RNG.Next(0, 2) % 2 == 0 ? ItemType.P99 : ItemType.Ak, owner);
        }
    }
}
