using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.NPCs;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.BehaviourTree;
using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Items;
using Assets.Scripts.Settings;
using Assets.Scripts.Utility;
using UnityEngine.AI;
namespace Assets.Scripts.Scenario
{
    public class TutorialScenario : ScenarioBase
    {
        private Difficulty difficulty = Difficulty.None;
        public Transform[] NPCSpawnPoints;

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

        protected override void Load()
        {
            base.Load();
            LoadStyle.SetDifficulty(difficulty);
           

            LoadRandom random = (LoadRandom)LoadStyle;
            random.Plein = true;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {

            Debug.Log(CurrentStage);
            if (!Started)
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
                Started = false;
            }


        }

        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }

        private void StartStage(Stage stage)
        {

            switch (stage)
            {
                case Stage.ShowSuspect:
                    SpawnNPC(true, DummyTargetPrefab, NPCSpawnPoints[0].position, NPCSpawnPoints[0].rotation);
              
                    break;
                case Stage.ShowCivilian:
                    break;
                case Stage.MultipleSuspects:
                    break;
                case Stage.Cover:
                    break;
                case Stage.Practise:
                    break;

            }
            Started = true;
        }

        private void UpdateStage(Stage stage)
        {
            
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

        private bool StageEnded()
        {
            return Started && NPC.HostileNpcs.Count == 0;
        }
    }
}
