namespace Assets.Scripts.BehaviourTree.Branch
{
    /// <inheritdoc />
    /// <summary>
    /// Chain multiple child behaviours together and execute them one by one.
    /// Traverses in a random order. Fails when a child failed.
    /// </summary>
    public class RandomSequence : RandomBranch
    {
        protected override Status Update()
        {
            while (true)
            {
                if (Current == null) break;

                Status status = Current.Tick();

                // If child fails or keeps running, do the same
                if (status != Status.Success)
                {
                    return status;
                }

                // Move on to the next child in the sequence
                if (!MoveNext())
                {
                    // Reached end of sequence
                    return Status.Success;
                }
            }
            return Status.Invalid; // Unexpected loop exit
        }
    }
}