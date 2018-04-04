using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Saves data relevant to the player.
    /// </summary>
    public class Player : MonoBehaviour
    {
        private const float DEFAULT_HEALTH = 100;

        public Transform PlayerCameraEye;

        public float Health = DEFAULT_HEALTH;

        public static Player Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            if (PlayerCameraEye == null)
                throw new NullReferenceException("Player has no reference to its Camera (eye).");
        }

        /// <summary>
        /// Handles hit from NPC
        /// </summary>
        /// <param name="damage"></param>
        public void HandleHit(float damage)
        {
            if (Health <= 0) return;

            Health -= damage;

            if (Health <= 0) GameOver();
        }

        /// <summary>
        /// enables end game state
        /// </summary>
        private void GameOver()
        {

            Scenario.ScenarioBase.Instance.GameOver();
        }
    }
}