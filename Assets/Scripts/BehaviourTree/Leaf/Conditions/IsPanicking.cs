using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Determines if the character is nervous by checking its property.
    /// Fails when there is no NPC or when NPC isn't nervous.
    /// </summary>
    public class IsPanicking : Condition
    {
        public static bool playerShot = false;
        private DataModel model;

        public IsPanicking(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
            model = dataModel;
        }

        protected override bool CheckCondition()
        {
            bool hostile = model.Npc.IsHostile;
            bool nul = DataModel.Npc != null;
            bool isPanic = DataModel.Npc.IsPanicking;
            bool con = !hostile && (nul && (isPanic || playerShot));
            return con;
        }
    }
}