namespace Assets.Scripts.BehaviourTree.Branch
{
    /// <inheritdoc />
    /// <summary>
    /// Find and execute the first child that does not fail.
    /// Traverses from front to back. Fails when all children failed.
    /// </summary>
    public class Selector : Branch
    {
        protected override Status Update()
        {
            while (true)
            {
                if (Enumerator.Current == null) break;

                Status status = Enumerator.Current.Tick();

                // If child succeeds or keeps running, keep going
                if (status != Status.Failure) return status;

                // Move on to the next child in the sequence
                if (!Enumerator.MoveNext())
                {
                    // Reached end of sequence
                    return Status.Failure;
                }
            }
            return Status.Invalid; // Unexpected loop exit
        }

        protected override void Terminate(Status status)
        {
            base.Terminate(status);

            Enumerator.Dispose();
        }
    }
}