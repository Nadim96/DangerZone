namespace Assets.Scripts.BehaviourTree
{
    /// <summary>
    /// BT is a Behaviour Tree used by AI to simulate realistic decision making.
    /// </summary>
    public class BT
    {
        /// <summary>
        /// The root node of the Behaviour Tree.
        /// </summary>
        protected BTNode Root { get; set; }

        public BT(BTNode root)
        {
            Root = root;
        }

        /// <summary>
        /// Updates the Behaviour tree. Should be called regularly.
        /// </summary>
        public void Tick()
        {
            Root.Tick();
        }
    }
}