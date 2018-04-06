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
            ShowWorld,
            Movement,
            InspectWeapon,
            Cover,
            GoalExplention,
            Practise,
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
        private Difficulty difficulty = Difficulty.None;

        // Points for spawning of npcs
        public Transform[] NPCSpawnPoints;

        public GameObject UIRoot;

        // Ingame menu and dialog screen
        public GameObject IngameMenu;
        public Text IngameMenuText;
        public Text IngameMenuTextDetail;
        public GameObject IngameMenuStartButton;
        public GameObject StartButton;

        private float timer;
        private Vector3 startingPosition;

        protected override void Load()
        {
            SetDifficulty(Difficulty.None);
            base.Load();
        }

        protected override void Start()
        {
            base.Start();
            SetMenuEnabled(true);
            timer = 0;
        }

        protected override void Update()
        {
            UpdateStage(CurrentStage);

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
        /// Sets the text on the ingame menu
        /// </summary>
        /// <param name="text"></param>
        /// <param name="aditionalinfo"></param>
        private void SetMenuText(string text, string aditionalinfo)
        {
            IngameMenuText.text = text;
            IngameMenuTextDetail.text = aditionalinfo;
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
        /// Enables the menu start button
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMenuStart(bool enabled)
        {
            IngameMenuStartButton.SetActive(enabled);
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
                case Stage.None: break;
                case Stage.ShowWorld:
                    SetMenuText("Welkom bij dangerzone.", "");
                    SetMenuStart(false);
                    SetMenuEnabled(true);
                    break;
                case Stage.Movement:
                    SetMenuText("Het blauwe vierkant is het speelveld. Loop maar door de ruimte heen!", "");
                    SetMenuEnabled(true);
                    Vector3 loc = this.PlayerCameraEye.transform.position;
                    startingPosition = new Vector3(loc.x, loc.y, loc.z);
                    break;
                case Stage.InspectWeapon:
                    SetMenuText("Kijk maar naar je wapen deze heeft 1 magazijn(15 kogels!)", "Verder op staat een dummie, probeer op hem.");
                    SetMenuEnabled(true);
                    SpawnNPC(true, DummyTargetPrefab, NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
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
                case Stage.GoalExplention:
                    SpawnNPC(true, PersonTargetPrefabs[7], NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, PersonTargetPrefabs[7], NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[3].position, NPCSpawnPoints[3].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[4].position, NPCSpawnPoints[4].rotation);
                    break;
                case Stage.Practise:
                    base.Load();
                    base.Create();
                    base.Spawn();
                    break;

            }

        }

        /// <summary>
        /// Updates during the stage
        /// </summary>
        /// <param name="stage">the current stage being updated</param>
        private void UpdateStage(Stage stage)
        {
            switch (stage)
            {
                case Stage.None: break;
                case Stage.ShowWorld:
                    timer += Time.deltaTime;
                    if (timer > 5)
                    {
                        timer = 0;
                        EndStage(CurrentStage, StageEndReason.Succes);
                    }
                    break;
                case Stage.Movement:
                    Vector3 distance = this.PlayerCameraEye.transform.position - startingPosition;
                    if (distance.magnitude > 1) { 
                        EndStage(CurrentStage, StageEndReason.Succes);
                    }
                    break;
                case Stage.InspectWeapon:
                    timer += Time.deltaTime;

                    if (timer > 3 && NPC.HostileNpcs.Count != 0)
                    {
                        SetMenuEnabled(false);
                    }

                
                     if (NPC.HostileNpcs.Count == 0)
                    {
                        SetMenuText("Goed gedaan!", "");
                        SetMenuStart(true);
                        SetMenuEnabled(true);
                        EndStage(CurrentStage, StageEndReason.Succes);
                    }
                    break;
                case Stage.Cover:
                    break;
                case Stage.GoalExplention: break;
                case Stage.Practise:
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
                case Stage.None: break;
                case Stage.ShowWorld:
                    Play();
                    break;
                case Stage.Movement:
                    Play();
                    break;
                case Stage.InspectWeapon:
                    break;
                case Stage.Cover:
                    switch (reason)
                    {
                        case StageEndReason.Succes:
                            Scenario.GameOver.instance.SetEndscreen(true);
                            IngameMenuTextDetail.text = "";
                            StartButton.SetActive(true);
                            break;
                        case StageEndReason.AgentDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
                            IngameMenuTextDetail.text = "";
                            break;
                        case StageEndReason.CivilianDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
                            IngameMenuTextDetail.text = "";

                            break;
                    }
                    SetMenuEnabled(true);
                    break;

                case Stage.GoalExplention:
                    switch (reason)
                    {
                        case StageEndReason.Succes:
                            Scenario.GameOver.instance.SetEndscreen(true);
                            break;
                        case StageEndReason.AgentDied:
                            IngameMenuTextDetail.text = "";
                            Scenario.GameOver.instance.SetEndscreen(false);
                            break;
                        case StageEndReason.CivilianDied:
                            Scenario.GameOver.instance.SetEndscreen(false);
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

            // Set events
            t.NPC.OnDeath += OnNPCDeath;
            trans.rotation = rotation;
        }

        /// <summary>
        /// Event triggered when a npc dies
        /// </summary>
        /// <param name="npc">the npc that died</param>
        /// <param name="hitmessage">the info about the hit</param>
        private void OnNPCDeath(NPC npc, HitMessage hitmessage)
        {
            timer = 0;
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
