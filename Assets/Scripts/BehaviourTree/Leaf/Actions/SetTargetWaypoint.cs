namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Sets the moveposition of the NPC to a waypoint, and advances to the next
    /// waypoint if necessary. Loops around.
    /// </summary>
    public class SetTargetWaypoint : Leaf
    {
        public SetTargetWaypoint(DataModel dataModel) : base(dataModel)
        {
        }

        protected override Status Update()
        {
            if (DataModel.Npc.CurrentWaypoint >= DataModel.Npc.Waypoints.Count - 1) //go back to first waypoint
                DataModel.Npc.CurrentWaypoint = 0;
            else
                DataModel.Npc.CurrentWaypoint++;

            DataModel.MovePosition = DataModel.Npc.Waypoints[DataModel.Npc.CurrentWaypoint].Position;
            return Status.Success;
        }
    }
}