namespace Assets.Scripts.BehaviourTree.Branch
{
    /// <inheritdoc />
    /// <summary>
    /// Find and execute the first child that does not fail.
    /// Traverses in a random order.
    /// </summary>
    public class RandomSelector : RandomBranch
    {
        protected override Status Update()
        {
            while (true)
            {
                if (Current == null) break;

                Status status = Current.Tick();

                // If child succeeds or keeps running, keep going
                if (status != Status.Failure)
                {
                    return status;
                }

                return Status.Failure;
            }
            return Status.Invalid; // Unexpected loop exit
        }
    }
}