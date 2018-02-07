using System.Collections.Generic;
using Assets.Scripts.Utility;

namespace Assets.Scripts.BehaviourTree.Branch
{
    /// <summary>
    /// Selects a random branch
    /// </summary>
    public class RandomBranch : Branch
    {
        private readonly List<int> _possibleIndices;
        protected BTNode Current;

        public RandomBranch()
        {
            _possibleIndices = new List<int>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            MoveNext(); // Randomly select first item of sequence
        }

        /// <summary>
        /// Get random item in _possibleIndices, use it as an index for children, 
        /// and remove used index from _possibleIndices.
        /// </summary>
        /// <returns></returns>
        protected bool MoveNext()
        {
            if (_possibleIndices.Count == 0) return false;

            int numberChosen = RNG.Next(0, GetTotal());

            //loop through possibleindices till correct index is found
            int index = 0;

            for (int i = 0; i <= numberChosen;)
            {
                index++;
                i += _possibleIndices[index - 1];
            }

            Current = Children[index - 1];

            return true;
        }

        private int GetTotal()
        {
            int total = 0;
            foreach(int i in _possibleIndices)
                total += i;
            return total;
        }

        /// <inheritdoc />
        public override void Add(BTNode child) 
        {
            Add(child, 1);
        }


        /// <inheritdoc cref="Branch.Add(BTNode)"/>
        /// <param name="child"></param>
        /// <param name="randomChange"></param>
        public void Add(BTNode child, int randomChange)
        {
            base.Add(child);

            _possibleIndices.Add(randomChange);
        }
    }
}
