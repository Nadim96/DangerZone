namespace Assets.Scripts.BehaviourTree.Branch
{
    /// <inheritdoc />
    /// <summary>
    /// Chain multiple child behaviours together and execute them one by one.
    /// Traverses from front to back. Fails when a child failed.
    /// </summary>
    public class Sequence : Branch
    {
        protected override Status Update()
        {
            while (true)
            {
                if (Enumerator.Current == null) break;

                Status status = Enumerator.Current.Tick();

                // If child fails or keeps running, do the same
                if (status != Status.Success)
                {
                    return status;
                }

                // Move on to the next child in the sequence
                if (!Enumerator.MoveNext())
                {
                    // Reached end of sequence
                    return Status.Success;
                }
            }
            return Status.Invalid; // Unexpected loop exit
        }
    }
}