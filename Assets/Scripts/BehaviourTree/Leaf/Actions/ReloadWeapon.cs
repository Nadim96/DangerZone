using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Will reload the weapon when possible and necessary. Always succeeds.
    /// </summary>
    public class ReloadWeapon : Leaf
    {
        private float _timeReloadStarted;
        private bool _startedReloading;
        private RangedWeapon _weapon;

        public ReloadWeapon(DataModel dataModel) : base(dataModel)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (DataModel.Npc != null && DataModel.Npc.Item != null)
            {
                _weapon = DataModel.Npc.Item as RangedWeapon;
                if (_weapon != null && _weapon.Ammo == 0)
                {
                    _weapon.Reload();
                    _timeReloadStarted = Time.time;
                    _startedReloading = true;
                }
            }
        }

        protected override Status Update()
        {
            if (!_startedReloading)
            {
                return Status.Success; // Assume we can't reload or that it isn't necessary
            }

            bool result = Time.time >= _timeReloadStarted + _weapon.ReloadTime;

            return result ? Status.Success : Status.Running;
        }

        protected override void Terminate(Status status)
        {
            _timeReloadStarted = 0;
            _startedReloading = false;
            _weapon = null;
        }
    }
}
