using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Settings;
using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// Handle all actions during a scenario
    /// </summary>
    public class BunkerScenario : ScenarioBase
    {
        public GameObject PlayAreaLocationCorner;
        public GameObject PlayAreaLocationLevelWall;
        public GameObject PlayAreaLocationLevelMiddle;
        // Enable correct cover
        GameObject g;
        /// <summary>
        /// list of all lights in level
        /// </summary>
        public Light[] lights;

        /// <summary>
        /// enable or disable lights
        /// </summary>
        private void SetLights()
        {
            lights.ForEach(x => x.enabled = ScenarioSettings.Lights);
        }
        protected override void Start()
        {
            base.Start();
            SetLights();
            SetSpawnArea();

        }

        protected override void Load()
        {
            LoadStyle.SetDifficulty(Difficulty.Easy);
            base.Load();          
        }

        public override void Play()
        {
            if (g != null) g.SetActive(false);
            base.Play();
            SetSpawnArea();
        }

        /// <summary>
        /// sets the player to selected spawnarea
        /// </summary>
        /// <param name="levelType"></param>
        private void SetSpawnArea()
        {
            // Set playarea position
            GameObject playerArea = GameObject.Find("[CameraRig]");

            if (!playerArea)
                throw new NullReferenceException("Unable to find [PlayerArea] GameObject.");

            if (!PlayAreaLocationCorner || !PlayAreaLocationLevelWall || !PlayAreaLocationLevelMiddle)
                throw new NullReferenceException("Unable to find the different PlayAreaLocation objects.");

            System.Random r = new System.Random();

            int location = r.Next(0, 3);
            switch (location)
            {
                case 0:
                    g = PlayAreaLocationCorner;
                    break;
                case 1:
                    g = PlayAreaLocationLevelWall;
                    break;
                case 2:
                    g = PlayAreaLocationLevelMiddle;
                    break;
                default:
                    g = PlayAreaLocationCorner;
                    break;
            }
            playerArea.transform.position = g.transform.position;

            g.SetActive(true);
        }
    }
}