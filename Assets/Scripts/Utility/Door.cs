using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <inheritdoc />
    /// <summary>
    /// This script is placed on a door object. 
    /// The door will be opened when shot at.
    /// </summary>
    public class Door : MonoBehaviour
    {
        public bool IsOpen;
        public bool CanOpen;

        public static Door instance;
        private bool _previousIsOpen;

        private Animator _animator;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Used to open the door using the inspector
            if (CanOpen && IsOpen != _previousIsOpen)
            {
                _previousIsOpen = IsOpen;
                SetOpen(IsOpen);
            }
        }

        /// <summary>
        /// Gets triggered to open when hit by a gun.
        /// </summary>
        /// <param name="hitMessage"></param>
        public void OnHit(HitMessage hitMessage)
        {
            if (CanOpen)
            {
                SetOpen(true);
            }
        }

        /// <summary>
        /// Plays the correct open/close animation.
        /// </summary>
        /// <param name="isOpen"></param>
        public void SetOpen(bool isOpen)
        {
            _animator.SetBool("IsOpen", isOpen);

            IsOpen = isOpen;
        }
    }
}