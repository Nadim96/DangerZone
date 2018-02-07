using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.BehaviourTree.Branch
{
    /// <inheritdoc cref="BTNode" />
    /// <summary>
    /// Base class for all composite AiBehaviours.
    /// </summary>
    public abstract class Branch : BTNode, IEnumerable<BTNode>
    {
        /// <summary>
        /// Contains all child nodes of this composite.
        /// </summary>
        protected List<BTNode> Children = new List<BTNode>();

        /// <summary>
        /// Enumerator of the children.
        /// </summary>
        protected List<BTNode>.Enumerator Enumerator;

        /// <summary>
        /// Initializes the branch.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Enumerator = Children.GetEnumerator();
            Enumerator.MoveNext(); // Move to first element
        }

        /// <summary>
        /// Adds a child to the back of the list.
        /// </summary>
        /// <param name="child"></param>
        public virtual void Add(BTNode child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// Removes the specified child if found.
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(BTNode child)
        {
            Children.Remove(child);
        }

        /// <summary>
        /// Removes all children from this composite.
        /// </summary>
        public void ClearChildren()
        {
            Children.Clear();
        }

        /// <summary>
        /// Terminates the branch by aborting each child.
        /// </summary>
        /// <param name="status"></param>
        protected override void Terminate(Status status)
        {
            foreach (BTNode child in Children)
            {
                if (child.IsRunning)
                {
                    child.Abort();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<BTNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}