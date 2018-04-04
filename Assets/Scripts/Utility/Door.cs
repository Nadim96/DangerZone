using System;
using System.Collections;
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
        float timeLeft = 2.00f;
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

                if (timeLeft <= 0.00f)
                {
                    timeLeft = 0;
                    _animator.SetBool("IsOpen", true);
                    SetOpen(IsOpen);
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                    Debug.Log(Math.Round(timeLeft));
                }


            }
        }

        /// <summary>
        /// Gets triggered to open when hit by a gun.
        /// </summary>
        /// <param name="hitMessage"></param>
      /*  public void OnHit(HitMessage hitMessage)
        {
            if (CanOpen)
            {
                SetOpen(true);
            }
        }
        */

        /// <summary>
        /// Plays the correct open/close animation.
        /// </summary>
        /// <param name="isOpen"></param>
        public void SetOpen(bool isOpen)
        {
            IsOpen = isOpen;
        }
    }
}