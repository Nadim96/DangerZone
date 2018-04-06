using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Assets.Scripts.NPCs;
using Assets.Scripts.Items;
using Assets.Scripts.Utility;
using Assets.Scripts.Settings;
using Assets.Scripts.BehaviourTree;

namespace Assets.Scripts.Scenario
{
    public class TutorialScenario : ScenarioBase
    {
        /// <summary>
        /// The stages that are in the tutorial
        /// </summary>
        private enum Stage
        {
            None,
            ShowPeople,
            Cover,
            Practise
        }

        /// <summary>
        /// Reasons why a stage has ended
        /// </summary>
        private enum StageEndReason
        {
            AgentDied,
            CivilianDied,
            Succes
        }

        // Current stage
        private Stage CurrentStage = Stage.None;
        private Difficulty difficulty = Difficulty.Plein;

        // Points for spawning of npcs
        public Transform[] NPCSpawnPoints;

        public GameObject UIRoot;

        // Ingame menu and dialog screen
        public GameObject IngameMenu;
        public Text IngameMenuText;
        public Text IngameMenuTextDetail;
        public String[] MenuMessages;
        public GameObject StartButton;

        protected override void Load()
        {
            SetDifficulty(Difficulty.Plein);
            base.Load();
        }

        protected override void Start()
        {
            base.Start();
            IngameMenuText.text = MenuMessages[0];
            IngameMenuTextDetail.text = MenuMessages[5] + '\n' + MenuMessages[6];
            SetMenuEnabled(true);
        }

        protected override void Update()
        {
            if (StageEnded())
            {
                EndStage(CurrentStage, StageEndReason.Succes);
            }

            if (Started && !AttackTriggered)
            {
                if (Time.time > ScenarioStartedTime + timeBeforeAttack)
                {
                    AttackTriggered = true;
                }
            }
        }

        /// <summary>
        /// Gets triggered when the ingame menu's "Play" button is activated
        /// </summary>
        public void OnMenuPlayButton()
        {
            // If not practise stage then just play the next stage
            if (CurrentStage != Stage.Practise - 1 && CurrentStage != Stage.Practise)
            {
                Play();
            }
            else
            {
                //reset for the practise stage
                Started = false;
                ClearNPCS();
                SetMenuEnabled(false);
                Time.timeScale = 1f;
                Scenario.GameOver.instance.HideEndScreen();
            }
        }

        /// <summary>
        /// Gets triggered when the restart button on the ground is activated and restarts the stage
        /// </summary>
        public void OnRestartButton()
        {
            Play();
            SetMenuEnabled(false);
        }

        /// <summary>
        /// Toggles the UI on the ground
        /// </summary>
        public override void SetIngameUIVisible()
        {
            EnableIngameMenu = true;
            IngameUI.SetActive(true);
        }

        /// <summary>
        /// Starts the next stage
        /// </summary>
        public override void Play()
        {
            // Increments to the next stage if not practise
            if (CurrentStage != Stage.Practise)
            {
                CurrentStage++;
            }

            // Sets the stage
            ClearNPCS();
            Started = true;
            Time.timeScale = 1f;
            SetMenuEnabled(false);
            AttackTriggered = false;
            StartStage(CurrentStage);
            ScenarioStartedTime = Time.time;
            PlayerCameraEye.GetComponent<Player.Player>().Health = 100;
            timeBeforeAttack = RNG.NextFloat(minTimeElapsedBeforeAttack, maxTimeElapsedBeforeAttack);
        }

        /// <summary>
        /// Clears all npcs of the map
        /// </summary>
        private void ClearNPCS()
        {
            foreach (Target t in Targets)
            {
                t.Destroy();
            }
            Targets.Clear();
            NPC.Npcs.Clear();
            NPC.HostileNpcs.Clear();
        }

        /// <summary>
        /// Enables or disables the ingame menu
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMenuEnabled(bool enabled)
        {
            IngameMenu.SetActive(enabled);
            EnableIngameMenu = enabled;
        }

        /// <summary>
        /// Stops the scenario 
        /// </summary>
        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }

        /// <summary>
        /// Triggers game over state
        /// </summary>
        public override void GameOver()
        {
            EndStage(CurrentStage, StageEndReason.AgentDied);
            Scenario.GameOver.instance.SetEndscreen(false);
            UIRoot.SetActive(true);
            Time.timeScale = 0.0f;
            SetMenuEnabled(true);
        }

        /// <summary>
        /// Starts a stage
        /// </summary>
        /// <param name="stage">the stage to be started</param>
        private void StartStage(Stage stage)
        {
            Scenario.GameOver.instance.HideEndScreen();
            //Handles every stage
            switch (stage)
            {
                case Stage.ShowPeople:
                    SpawnNPC(true, PersonTargetPrefabs[7], NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, PersonTargetPrefabs[7], NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[3].position, NPCSpawnPoints[3].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[4].position, NPCSpawnPoints[4].rotation);
                    break;
                case Stage.Cover:
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[3].position, NPCSpawnPoints[3].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[4].position, NPCSpawnPoints[4].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[5].position, NPCSpawnPoints[5].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[6].position, NPCSpawnPoints[6].rotation);
                    break;
                case Stage.Practise:
                    base.Load();
                    base.Create();
                    base.Spawn();
                    break;

            }
           
        }

        /// <summary>
        /// Handles the ending of a stage
        /// </summary>
        /// <param name="stage">stage to be ended</param>
        /// <param name="reason">reason why the stage ended</param>
        private void EndStage(Stage stage, StageEndReason reason)
        {
            Time.timeScale = 0f;
            Started = false;

            // If the reason is that tehe agent failed reset the stage back
            if (reason == StageEndReason.AgentDied || reason == StageEndReason.CivilianDied)
            {
                if (CurrentStage > 0)
                {
                    CurrentStage--;
                }
            }

            // Handle every case
            switch (stage)
            {
                case Stage.ShowPeople:
                    switch (reason)
                    {
                        case StageEndReason.Succes:
                            Scenario.GameOver.instance.SetEndscreen(true);
                            IngameMenuText.text = MenuMessages[2];
                            IngameMenuTextDetail.text = MenuMessages[7] + '\n' + MenuMessages[8];
                            break;
                        case StageEndReason.AgentDied:
                            IngameMenuText.text = MenuMessages[3];
                            IngameMenuTextDetail.text = "";
                            Scenario.GameOver.instance.SetEndscreen(false);
                            break;
                        case StageEndReason.CivilianDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
                            IngameMenuText.text = MenuMessages[1];
                            IngameMenuTextDetail.text = "";
                            break;
                    }
                    SetMenuEnabled(true);
                    break;
                case Stage.Cover:
                    switch (reason)
                    {
                        case StageEndReason.Succes:
                            Scenario.GameOver.instance.SetEndscreen(true);
                            IngameMenuText.text = MenuMessages[4];
                            IngameMenuTextDetail.text = "";
                            StartButton.SetActive(true);
                            break;
                        case StageEndReason.AgentDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
                            IngameMenuText.text = MenuMessages[3];
                            IngameMenuTextDetail.text = "";
                            break;
                        case StageEndReason.CivilianDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
                            IngameMenuText.text = MenuMessages[1];
                            IngameMenuTextDetail.text = "";

                            break;
                    }
                    SetMenuEnabled(true);
                    break;
                case Stage.Practise:
                    switch (reason)
                    {
                        case StageEndReason.Succes:
                            Scenario.GameOver.instance.SetEndscreen(true);
                            break;
                        case StageEndReason.AgentDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
                            break;
                        case StageEndReason.CivilianDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Spawn an npc
        /// </summary>
        /// <param name="hostile">wether the npc is hostile</param>
        /// <param name="type">what kind of npc is to be spawned</param>
        /// <param name="location">location of the npc</param>
        /// <param name="rotation">rotation of the npc</param>
        private void SpawnNPC(bool hostile, Transform type, Vector3 location, Quaternion rotation)
        {
            // Create npc
            TargetNpc t = new TargetNpc();
            t.Difficulty = difficulty;
            t.ItemType = ItemType.P99;
            t.Position = location;
            t.IsHostile = hostile;

            Targets.Add(t);

            // Spawn npc
            Transform trans = t.Spawn(type);

            t.NPC.OnNPCDeathEvent += OnNPCDeath;
            trans.rotation = rotation;
        }

        /// <summary>
        /// Event triggered when a npc dies
        /// </summary>
        /// <param name="npc">the npc that died</param>
        /// <param name="hitmessage">the info about the hit</param>
        private void OnNPCDeath(NPC npc, HitMessage hitmessage)
        {
            // Check if the npc was killed by a player and was a neutral npc
            if (!npc.IsHostile && hitmessage.IsPlayer)
            {
                // End the current stage
                EndStage(CurrentStage, StageEndReason.CivilianDied);
            }
        }

        /// <summary>
        /// Sets the current diffeculty
        /// </summary>
        /// <param name="difficulty">the difficulty</param>
        private void SetDifficulty(Difficulty difficulty)
        {
            this.difficulty = difficulty;
            LoadStyle.SetDifficulty(difficulty);
        }

        /// <summary>
        /// Check to see if a stage ended
        /// </summary>
        /// <returns>whether the stage has ended</returns>
        private bool StageEnded()
        {
            return Started && NPC.HostileNpcs.Count == 0;
        }

    }
}
