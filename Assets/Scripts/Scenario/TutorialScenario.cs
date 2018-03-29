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
        private enum Stage
        {
            None,
            ShowPeople,
            Cover,
            Practise
        }

        private enum StageEndReason
        {
            AgentDied,
            CivilianDied,
            Succes
        }

        private Stage CurrentStage = Stage.None;
        private Difficulty difficulty = Difficulty.Plein;

        public Transform[] NPCSpawnPoints;
        public GameObject UIRoot;

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
          
            if (!CanStartStage())
            {
                UpdateStage(CurrentStage);
            }

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

        public void OnMenuPlayButton()
        {
            if (CurrentStage != Stage.Practise - 1 && CurrentStage != Stage.Practise)
            {
                Play();
            }
            else
            {
                Started = false;
                ClearNPCS();
                SetMenuEnabled(false);
                Time.timeScale = 1f;
                Scenario.GameOver.instance.HideEndScreen();
            }
        }

        public void OnRestartButton()
        {
            Play();
            SetMenuEnabled(false);
        }

        public override void SetIngameUIVisible()
        {
            EnableIngameMenu = true;
            IngameUI.SetActive(true);
        }

        public override void Play()
        {
            if (CurrentStage != Stage.Practise)
            {
                CurrentStage++;
            }

            ClearNPCS();
            StartStage(CurrentStage);
            Started = true;
            SetMenuEnabled(false);
            AttackTriggered = false;
            PlayerCameraEye.GetComponent<Player.Player>().Health = 100;
            Time.timeScale = 1f;
            ScenarioStartedTime = Time.time;
            timeBeforeAttack = RNG.NextFloat(minTimeElapsedBeforeAttack, maxTimeElapsedBeforeAttack);
        }

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


        public void SetMenuEnabled(bool enabled)
        {
            IngameMenu.SetActive(enabled);
            EnableIngameMenu = enabled;
        }

        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }

        public override void GameOver()
        {
            EndStage(CurrentStage, StageEndReason.AgentDied);
            Scenario.GameOver.instance.SetEndscreen(false);
            UIRoot.SetActive(true);
            Time.timeScale = 0.0f;
            SetMenuEnabled(true);
        }

        private void StartStage(Stage stage)
        {
            Scenario.GameOver.instance.HideEndScreen();
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

        private void UpdateStage(Stage stage)
        {

        }

        private void EndStage(Stage stage, StageEndReason reason)
        {
            Time.timeScale = 0f;
            Started = false;

            if (reason == StageEndReason.AgentDied || reason == StageEndReason.CivilianDied)
            {
                if (CurrentStage > 0)
                {
                    CurrentStage--;
                }
            }

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

        private void SpawnNPC(bool hostile, Transform type, Vector3 location, Quaternion rotation)
        {
            TargetNpc t = new TargetNpc();
            t.Difficulty = difficulty;
            t.ItemType = ItemType.P99;
            t.Position = location;
            t.IsHostile = hostile;

            Targets.Add(t);
            Transform trans = t.Spawn(type);
            t.NPC.OnNPCDeathEvent += OnNPCDeath;
            trans.rotation = rotation;
        }

        private void OnNPCDeath(NPC npc, HitMessage hitmessage)
        {
            if (!npc.IsHostile && hitmessage.IsPlayer)
            {
                EndStage(CurrentStage, StageEndReason.CivilianDied);
            }
        }

        private void SetDifficulty(Difficulty d)
        {
            this.difficulty = d;
            LoadStyle.SetDifficulty(d);
        }

        private bool StageEnded()
        {
            return Started && NPC.HostileNpcs.Count == 0;
        }

        private bool CanStartStage()
        {
            return !Started;
        }
    }
}
