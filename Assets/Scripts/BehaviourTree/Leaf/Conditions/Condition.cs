using System;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// A condition is used for checking if a datamodel/npc/world state is true. 
    /// </summary>
    public abstract class Condition : Leaf
    {
        public enum Mode
        {
            /// <summary>
            /// Instant Check will run only once and instantly return success or failure.
            /// </summary>
            InstantCheck,

            /// <summary>
            /// Monitoring mode will keep running as long as its condition is true. It can never succeed, only run or fail.
            /// </summary>
            Monitoring
        }

        private readonly Mode _mode;

        /// <summary>
        /// This will invert the output of the condition.
        /// For instant check this will be boolean negation, while with Monitoring mode it will keep running
        /// as long as its condition is false (= output of CheckCondition) if negation is true.
        /// </summary>
        private readonly bool _negate;

        protected Condition(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) : base(dataModel)
        {
            _negate = negate;
            _mode = mode;
        }

        /// <inheritdoc />
        /// <summary>
        /// The Condition class will handle the updating of extending classes and should not be overriden.
        /// </summary>
        /// <returns></returns>
        protected override Status Update()
        {
            bool output = CheckCondition();

            if (_negate)
            {
                output = !output;
            }

            switch (_mode)
            {
                case Mode.InstantCheck:
                    return output ? Status.Success : Status.Failure;
                case Mode.Monitoring:
                    return output ? Status.Running : Status.Failure;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Every class extending Condition should implement this method to check for its condition.
        /// </summary>
        /// <returns></returns>
        protected abstract bool CheckCondition();
    }
}