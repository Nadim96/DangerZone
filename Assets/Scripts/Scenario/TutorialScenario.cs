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

        private Stage CurrentStage = Stage.None;
        private Difficulty difficulty = Difficulty.Plein;

        private const float COOLDOWN = 1;
        private float AfterStageCoolDown;

        public Transform[] NPCSpawnPoints;
        public GameObject UIRoot;

        public Text IngameMenuText;
        public String GameOverMessage;
        public String[] MenuMessages;
        private int MenuMessageIndex = 0;

        protected override void Load()
        {
            SetDifficulty(Difficulty.Plein);
            base.Load();

        }

        protected override void Start()
        {
            base.Start();
            IngameMenuText.text = MenuMessages[MenuMessageIndex++];
        }

        protected override void Update()
        {
            AfterStageCoolDown -= Time.deltaTime;

            if (!CanStartStage())
            {
                UpdateStage(CurrentStage);
            }
            else
            {
                SetMenuEnabled(true);
            }

            if (StageEnded())
            {
                EndStage(CurrentStage);
            }

            if (Started && !AttackTriggered)
            {
                if (Time.time > ScenarioStartedTime + timeBeforeAttack)
                {
                    AttackTriggered = true;
                }
            }

            if (EnableIngameMenu && Time.time > ScenarioStartedTime + 2) //always enable after 2 second
            {
                IngameUI.SetActive(true);
                EnableIngameMenu = false;
            }
        }

        public override void Play()
        {
            foreach (Target t in Targets)
            {
                t.Destroy();
            }
            Targets.Clear();

            if (CurrentStage != Stage.Practise)
            {
                CurrentStage++;
            }

            StartStage(CurrentStage);
            Started = true;
            SetMenuEnabled(false);
            AttackTriggered = false;
            PlayerCameraEye.GetComponent<Player.Player>().Health = 100;

            if (MenuMessageIndex < MenuMessages.Count())
            {
                IngameMenuText.text = MenuMessages[MenuMessageIndex++];
            }

            Time.timeScale = 1f;
            ScenarioStartedTime = Time.time;
            timeBeforeAttack = RNG.NextFloat(minTimeElapsedBeforeAttack, maxTimeElapsedBeforeAttack);
        }


        public void SetMenuEnabled(bool enabled)
        {
            EnableIngameMenu = enabled;
            IngameUI.SetActive(enabled);
        }

        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }

        public override void GameOver()
        {
            Scenario.GameOver.instance.SetEndscreen(false);
            Time.timeScale = 0.0f;
            UIRoot.SetActive(true);

            EndStage(CurrentStage);
            if (CurrentStage > 0)
            {
                CurrentStage--;
                MenuMessageIndex--;
            }
            IngameMenuText.text = GameOverMessage;
            SetMenuEnabled(true);
        }

        private void StartStage(Stage stage)
        {
            Scenario.GameOver.instance.HideEndScreen();
            switch (stage)
            {
                case Stage.ShowPeople:
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[3].position, NPCSpawnPoints[3].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[4].position, NPCSpawnPoints[4].rotation);
                    break;
                case Stage.Cover:
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[3].position, NPCSpawnPoints[3].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[4].position, NPCSpawnPoints[4].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[5].position, NPCSpawnPoints[5].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[6].position, NPCSpawnPoints[6].rotation);
                    break;
                case Stage.Practise:
                    break;

            }

        }

        private void UpdateStage(Stage stage)
        {

        }

        private void EndStage(Stage stage)
        {
            Started = false;
            Time.timeScale = 0f;

            switch (stage)
            {
                case Stage.ShowPeople:
                    Scenario.GameOver.instance.SetEndscreen(true);
                    break;
                case Stage.Cover:
                    if (PlayerCameraEye.GetComponent<Player.Player>().Health == 0)
                    {
                        Scenario.GameOver.instance.SetEndscreen(true);
                    }
                    break;
                case Stage.Practise:
                    Scenario.GameOver.instance.SetEndscreen(true);
                    break;

            }
            StartAfterRoundCooldown();
        }

        private void StartAfterRoundCooldown()
        {
            this.AfterStageCoolDown = COOLDOWN;
        }

        private void SpawnNPC(bool hostile, Transform type, Vector3 location, Quaternion rotation)
        {
            TargetNpc t = new TargetNpc();
            t.Difficulty = difficulty;
            t.IsHostile = hostile;
            t.Position = location;
            t.ItemType = ItemType.P99;

            Targets.Add(t);
            Transform trans = t.Spawn(type);
            trans.rotation = rotation;
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
            return !Started && AfterStageCoolDown < 0;
        }
    }
}
