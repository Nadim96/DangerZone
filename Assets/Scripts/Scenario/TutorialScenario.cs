using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.BehaviourTree;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    public class TutorialScenario : ScenarioBase
    {
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
            LoadStyle.SetDifficulty(Difficulty.Plein);

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
                CurrentStage++;
                StartStage(CurrentStage);
            } else
            {
                UpdateStage(CurrentStage);
            }

            if (StageEnded())
            {

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
        }

        private void UpdateStage(Stage stage)
        {

        }

        private bool StageEnded()
        {

            return false;
        }
    }
}
