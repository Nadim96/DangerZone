using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
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
            ShowSuspect,
            ShowCivilian,
            MultipleSuspects,
            Cover,
            Practise
        }

        private Stage CurrentStage = Stage.None;
        private Difficulty difficulty = Difficulty.None;

        private const float COOLDOWN = 1;
        private float AfterStageCoolDown;

        public Transform[] NPCSpawnPoints;
        public Transform CoverPosition;
        public Transform CameraRig;

        protected override void Load()
        {
            base.Load();
            SetDifficulty(Difficulty.None);
           

            LoadRandom random = (LoadRandom)LoadStyle;
            random.Plein = true;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            AfterStageCoolDown -= Time.deltaTime;
            if (CanStartStage())
            {
                if (CurrentStage != Stage.Practise)
                {
                    CurrentStage++;
                }
                StartStage(CurrentStage);
            } else
            {
                UpdateStage(CurrentStage);
            }

            if (StageEnded())
            {
                EndStage(CurrentStage);
            }
        }

        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }

        private void StartStage(Stage stage)
        {
            Scenario.GameOver.instance.HideEndScreen();
            switch (stage)
            {
                case Stage.ShowSuspect:
                    SpawnNPC(true, DummyTargetPrefab, NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    break;
                case Stage.ShowCivilian:
                    SpawnNPC(true, DummyTargetPrefab, NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[5].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[6].position, NPCSpawnPoints[2].rotation);
                    break;
                case Stage.MultipleSuspects:
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[3].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[4].position, NPCSpawnPoints[2].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[5].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[6].position, NPCSpawnPoints[2].rotation);
                    break;
                case Stage.Cover:
                    CameraRig.position = CoverPosition.position;
                    SetDifficulty(Difficulty.Plein);
                    SpawnNPC(true, GetRandomNpc(), NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[1].position, NPCSpawnPoints[1].rotation);
                    SpawnNPC(false, GetRandomNpc(), NPCSpawnPoints[2].position, NPCSpawnPoints[2].rotation);
                    break;
                case Stage.Practise:
                    break;

            }
            Started = true;
        }

        private void UpdateStage(Stage stage)
        {
            
        }

        private void EndStage(Stage stage)
        {
            Started = false;
            foreach (Target t in Targets)
            {
                t.Destroy();
            }
            Targets.Clear();

            switch (stage)
            {
                case Stage.ShowSuspect:
                    Scenario.GameOver.instance.SetEndscreen(true);
                    break;
                case Stage.ShowCivilian:
                    Scenario.GameOver.instance.SetEndscreen(true);
                    break;
                case Stage.MultipleSuspects:
                    Scenario.GameOver.instance.SetEndscreen(true);
                    break;
                case Stage.Cover:
                    Scenario.GameOver.instance.SetEndscreen(true);
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
            
            Targets.Add(t);
            Transform trans = t.Spawn(type);
            trans.rotation = rotation;
        }

        private void SetDifficulty(Difficulty difficulty)
        {
            this.difficulty = difficulty;
            LoadStyle.SetDifficulty(difficulty);
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
