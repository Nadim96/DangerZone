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
            SetSpawnArea(ScenarioSettings.LevelType);

        }

        protected override void Load()
        {
            LoadStyle.SetDifficulty(Difficulty.Easy);
            base.Load();
        }

        /// <summary>
        /// sets the player to selected spawnarea
        /// </summary>
        /// <param name="levelType"></param>
        private void SetSpawnArea(LevelType levelType)
        {
            // Set playarea position
            GameObject playerArea = GameObject.Find("[CameraRig]");

            if (!playerArea)
                throw new NullReferenceException("Unable to find [PlayerArea] GameObject.");

            if (!PlayAreaLocationCorner || !PlayAreaLocationLevelWall || !PlayAreaLocationLevelMiddle)
                throw new NullReferenceException("Unable to find the different PlayAreaLocation objects.");

            // Enable correct cover
            GameObject g;
            switch (levelType)
            {
                case LevelType.Corner:
                    g = PlayAreaLocationCorner;
                    break;
                case LevelType.Wall:
                    g = PlayAreaLocationLevelWall;
                    break;
                case LevelType.Middle:
                    g = PlayAreaLocationLevelMiddle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("levelType", levelType, null);
            }
            playerArea.transform.position = g.transform.position;

            g.SetActive(true);
        }
    }
}