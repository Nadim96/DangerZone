using System.Collections;
using Assets.Scripts.Audio;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.NPCs
{
    /// <summary>
    /// Bird that can jump around
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Bird : MonoBehaviour
    {
        private const string ANIM_PECK = "Pecking";
        private const string ANIM_JUMP = "Jump";
        private const string ANIM_FLYING = "Flying";

        private bool _isJumping;
        private bool _isFlying;

        private float _animTimer;
        private float _soundTimer;
        private int _rngnumber;

        private Animator _animator;

        private void Start()
        {
            // Starts the timers and initializes the animator
            _animTimer = RNG.NextFloat(0, 1);
            _soundTimer = RNG.NextFloat(0, 1);
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            // This only works if the bird is not flying away
            if (!_isFlying)
            {
                // Increase the timers and if it hits a treshhold, play an animation or a sound
                _animTimer += Time.deltaTime;
                if (_animTimer > 3)
                {
                    _rngnumber = RNG.Next(0, 3);
                    PlayAnimation(_rngnumber);
                    _animTimer = 0;
                }

                _soundTimer += Time.deltaTime;
                if (_soundTimer > 4)
                {
                    _rngnumber = RNG.Next(1, 7);
                    AudioController.PlayAudio(gameObject, AudioCategory.BirdChirp, 1f);
                    _soundTimer = 0;
                }

                // If the bird is currently jumping, move it forward
                if (_isJumping && _animTimer > .5 && _animTimer < 1.2)
                {
                    transform.position += transform.forward * Time.deltaTime;
                }
            }
            else
            {
                // If the bird is above a certain height, destroy it. Else, move it upwards.
                if (transform.position.y >= 75)
                {
                    Destroy(gameObject);
                }
                transform.position += transform.forward * Time.deltaTime * 10;
            }
        }

        /// <summary>
        /// play animations for bird
        /// </summary>
        /// <param name="number"></param>
        private void PlayAnimation(int number)
        {
            _isJumping = false;
            switch (number)
            {
                case 0:
                    _animator.SetTrigger(ANIM_PECK);
                    break;
                case 1:
                    Jump();
                    break;
            }
        }

        /// <summary>
        /// Jump in direction of destination
        /// </summary>
        public void Jump()
        {
            Vector3 destination = new Vector3(RNG.Next(-75, 75), 0.1f, RNG.Next(-75, 75));
            transform.LookAt(destination);
            _animator.SetTrigger(ANIM_JUMP);
            _isJumping = true;
        }

        /// <summary>
        /// make bird fly away when panic is called
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public IEnumerator Panic(float delay)
        {
            if (_isFlying) yield break;

            yield return new WaitForSeconds(delay); // Wait a few seconds as "reaction time"

            _animator.SetTrigger(ANIM_FLYING);

            transform.LookAt(new Vector3(RNG.Next(-40, 40), 100, RNG.Next(-40, 40)));
            AudioController.PlayAudio(gameObject, AudioCategory.BirdWingFlap, AudioController.DialogueVolume);
            _isFlying = true;
        }

        /// <summary>
        /// Play death animation and sound
        /// </summary>
        private void Death()
        {
            // Play an audio, destroy the object
            AudioController.PlayAudioAtPoint(transform.position, AudioCategory.BirdDeath,
                AudioController.DialogueVolume);
            Destroy(transform.gameObject);
        }

        /// <summary>
        /// react to a hit
        /// </summary>
        /// <param name="hitMessage"></param>
        public void OnHit(HitMessage hitMessage)
        {
            Death();
        }
    }
}