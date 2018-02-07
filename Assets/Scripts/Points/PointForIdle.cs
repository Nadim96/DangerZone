using UnityEngine;

namespace Assets.Scripts.Points
{
    /// <summary>
    /// collection of points NPC can idle at
    /// </summary>
    public class PointForIdle : MonoBehaviour
    {
        [SerializeField] private IdleAnimationType _animationToUse;

        public IdleAnimationType AnimationToUse
        {
            get { return _animationToUse; }
        }

        public bool Occupied { get; set; }
    }
}