namespace Assets.Scripts.BehaviourTree.Decorator
{
    /// <inheritdoc />
    /// <summary>
    /// Base class to add functionality to a node.
    /// </summary>
    public abstract class Decorator : BTNode
    {
        protected BTNode Child;

        protected Decorator(BTNode child)
        {
            Child = child;
        }

        protected override Status Update()
        {
            return Child.Tick();
        }
    }
}