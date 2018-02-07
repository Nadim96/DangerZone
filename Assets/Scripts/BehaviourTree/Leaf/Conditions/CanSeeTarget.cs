using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Raycast from owner Npc at EYE_HEIGHT to the target object center of mass, 
    /// which is assumed to be CENTER_OF_MASS_HEIGHT above target transform.
    /// </summary>
    public class CanSeeTarget : Condition
    {
        /// <summary>
        /// The eye height is higher than you would expect, we've done this to solve
        /// some line of sight issues we experienced.
        /// </summary>
        private const float EYE_HEIGHT = 1.6f;

        public CanSeeTarget(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            if (DataModel.Npc == null || DataModel.Target == null)
            {
                return false; // No entity to turn with or target to turn to
            }

            Vector3 eyePosOwner = DataModel.Npc.transform.position +
                                  new Vector3(0, EYE_HEIGHT, 0);

            Vector3 centerOfMassTarget = DataModel.Target.transform.position; //+
            //new Vector3(0, CENTER_OF_MASS_HEIGHT, 0);

            // Use layers to ignore self collision
            int defaultLayer = LayerMask.NameToLayer("Default");
            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

            DataModel.Npc.gameObject.layer = ignoreRaycastLayer;

            // Do linecast
            RaycastHit hit;
            bool result = Physics.Linecast(eyePosOwner, centerOfMassTarget, out hit);

            // Revert layer
            DataModel.Npc.gameObject.layer = defaultLayer;
            Debug.DrawLine(eyePosOwner, centerOfMassTarget, Color.white, 0.1f);


            bool condition = result && (hit.transform == DataModel.Target.transform ||
                                        hit.transform.root == DataModel.Target.transform);

            if (hit.transform != null)
            {
                //   Debug.Log(hit.transform.name);
            }

            return condition;
        }
    }
}