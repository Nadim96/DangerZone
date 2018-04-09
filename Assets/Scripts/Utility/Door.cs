using System;
using System.Collections;
using Assets.Scripts.Scenario;
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
        float timeLeft;
        public bool OpenTrigger;
        public bool IsOpen;
        public bool CanOpen;

        public static Door instance;
        private bool _previousIsOpen;

        private Animator _animator;

        public GameObject IngameMenu;
        public Text IngameMenuTextDetail;


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
            if (CanOpen && OpenTrigger)
            {
                if (timeLeft <= 0f)
                {
					DoorScenario.isOpen = true;
                    SetMenuEnabled(false);
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

            if (IsOpen == true)
            {
                OpenTrigger = true;
                timeLeft = 5.00f;
            }
        }

    }
}