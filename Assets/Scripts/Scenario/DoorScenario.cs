using System.Collections;
using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    public class DoorScenario : ScenarioBase
    {
        public Door Door;
        public static bool isOpen;

        protected override void Load()
        {
            Room.instance.Generate();
            LoadStyle.SetDifficulty(Difficulty.Door);
            base.Load();

            Door.CanOpen = true;
        }

        public override void Play()
        {
            StartCoroutine(PlayRoutine());
            isOpen = false;
        }

        private IEnumerator PlayRoutine()
        {
            //close door first to make sure the door is closed when generating level
            Door.SetOpen(false);
            Door.CanOpen = false;
            isOpen = false;
            yield return new WaitForSecondsRealtime(1);
            Stop(); //hammertime
            base.Play();
            Door.CanOpen = true;
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
