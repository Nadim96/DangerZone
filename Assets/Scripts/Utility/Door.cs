using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utility
{
    /// <inheritdoc />
    /// <summary>
    /// This script is placed on a door object. 
    /// The door will be opened when shot at.
    /// </summary>
    public class Door : MonoBehaviour
    {
        float timeLeft = 5.00f;
        public bool IsOpen;
        public bool CanOpen;

        public static Door instance;
        private bool _previousIsOpen;

        private Animator _animator;

        public GameObject IngameMenu;
        public Text IngameMenuText;
        public Text IngameMenuTextDetail;
        public String[] MenuMessages;
        public GameObject StartButton;



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
                    SetMenuEnabled(false);
                    _previousIsOpen = IsOpen;
                    SetOpen(IsOpen);
                    timeLeft = 5.00f;
                    _animator.SetBool("IsOpen", true);
                }
                else
                {
                    SetMenuEnabled(true);
                    timeLeft -= Time.deltaTime;
                    IngameMenuTextDetail.text = Math.Round(timeLeft).ToString();
                }
            }
        }

        public void SetMenuEnabled(bool enabled)
        {
            IngameMenu.SetActive(enabled);
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
            SetMenuEnabled(false);
            IsOpen = isOpen;
            _animator.SetBool("IsOpen", false);

        }
    }
}