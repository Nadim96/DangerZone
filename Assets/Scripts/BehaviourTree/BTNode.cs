namespace Assets.Scripts.BehaviourTree
{
    /// <summary>
    /// Node used in a Behaviour Tree (BT) for NPC.
    /// </summary>
    public abstract class BTNode
    {
        /// <summary>
        /// status that is currently active
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Checks if the BTNode is terminated
        /// </summary>
        public bool IsTerminated
        {
            get { return Status == Status.Success || Status == Status.Failure; }
        }

        /// <summary>
        /// Checks if the BTNode is running
        /// </summary>
        public bool IsRunning
        {
            get { return Status == Status.Running; }
        }


        protected BTNode()
        {
            Status = Status.Invalid;
        }

        /// <summary>
        /// Triggered once when BTNode is updated for the first time.
        /// </summary>
        protected virtual void Initialize()
        {
            Status = Status.Running;
        }

        /// <summary>
        /// Updates the behaviour. Should be called by the Tick method.
        /// </summary>
        /// <returns></returns>
        protected virtual Status Update()
        {
            return Status;
        }

        /// <summary>
        /// Public entry point for updating the BTNode. 
        /// Ensures correct call order for Initialize, Update and Terminate.
        /// </summary>
        /// <returns></returns>
        public Status Tick()
        {
            if (Status != Status.Running)
            {
                Initialize();
            }

            Status = Update();

            if (Status != Status.Running)
            {
                Terminate(Status);
            }

            return Status;
        }

        /// <summary>
        /// Resets this Node.
        /// </summary>
        public void Reset()
        {
            Status = Status.Invalid;
        }

        /// <summary>
        /// Set status to aborted and terminate node.
        /// </summary>
        public void Abort()
        {
            Terminate(Status.Aborted);
            Status = Status.Aborted;
        }

        /// <summary>
        /// Triggered once when BTNode stops for some reason.
        /// Ensures behaviour is stopped gracefully.
        /// </summary>
        /// <param name="status">Reason for termination</param>
        protected virtual void Terminate(Status status)
        {
        }
    }
}