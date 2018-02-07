namespace Assets.Scripts.BehaviourTree.Branch
{
    /// <inheritdoc />
    /// <summary>
    /// Executes all child behaviours at once. The conditions for success or failure is dependent on its policies.
    /// Note that this does not use actual multi-threading. 
    /// </summary>
    public class Parallel : Branch
    {
        /// <summary>
        /// the requirements for succeeding
        /// </summary>
        public enum Policy
        {
            RequireOne,
            RequireAll
        }

        /// <summary>
        /// Condition for success: Must all children pass for this node to pass? Or at least one?
        /// </summary>
        protected Policy SuccessPolicy { get; set; }

        /// <summary>
        /// Condition for failure: Must all children fail for this node to fail? Or at least one?
        /// </summary>
        protected Policy FailurePolicy { get; set; }

        public Parallel(Policy successPolicy, Policy failurePolicy)
        {
            SuccessPolicy = successPolicy;
            FailurePolicy = failurePolicy;
        }

        protected override Status Update()
        {
            int successCount = 0;
            int failureCount = 0;

            foreach (BTNode child in Children)
            {
                if (!child.IsTerminated)
                {
                    child.Tick();
                }

                switch (child.Status)
                {
                    case Status.Success:
                        successCount++;
                        if (SuccessPolicy == Policy.RequireOne)
                        {
                            return Status.Success;
                        }
                        break;
                    case Status.Failure:
                        failureCount++;
                        if (FailurePolicy == Policy.RequireOne)
                        {
                            return Status.Failure;
                        }
                        break;
                }
            }

            if (SuccessPolicy == Policy.RequireAll && successCount == Children.Count)
            {
                return Status.Success;
            }

            // First part of if-statement avoids an unending parallel node in edge-case.
            if (failureCount + successCount == Children.Count || 
                FailurePolicy == Policy.RequireAll && failureCount == Children.Count)
            {
                return Status.Failure;
            }

            return Status.Running;
        }
    }
}