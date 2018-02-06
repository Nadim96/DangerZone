using System;
using UnityEngine;

namespace Assets.Scripts.NPCs
{
    /// <summary>
    /// Handles all animation events of the NPC
    /// </summary>
    class AnimationEvents : MonoBehaviour
    {
        [SerializeField] private AudioClip _footstepAudioClip;

        private AudioSource _audioSource;

        private void Awake()
        {
            if (_footstepAudioClip == null)
                throw new NullReferenceException("AnimationEvents has no footstep sound selected.");

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.maxDistance = 50;
                _audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                _audioSource.spatialBlend = 1.0f;
            }
        }

        public void OnFootstep()
        {
            _audioSource.PlayOneShot(_footstepAudioClip, 0.05f);
        }
    }
}