using System.Collections;
using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    public class DoorScenario : ScenarioBase
    {
        public Door Door;

        protected override void Load()
        {
            Room.instance.Generate();
            LoadStyle.SetDifficulty(Difficulty.Door);
            base.Load();
            Door.IngameMenu.SetActive(false);
            Door.SetOpen(false);
            Door.CanOpen = true;
        }

        public override void Play()
        {
            StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            //close door first to make sure the door is closed when generating level
    
            yield return new WaitForSecondsRealtime(1);
            Stop(); //hammertime
            Door.SetOpen(false);
            Door.CanOpen = true;
            base.Play();
        }

        public override void Stop()
        {
            Door.SetOpen(false);
            Door.CanOpen = false;

            Room.instance.DeleteRoom();
            base.Stop();
        }
    }
}
