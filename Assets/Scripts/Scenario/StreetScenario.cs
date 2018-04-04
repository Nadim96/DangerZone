using Assets.Scripts.BehaviourTree;
using UnityEngine;
namespace Assets.Scripts.Scenario
{
    public class StreetScenario : ScenarioBase
    {
        protected override void Load()
        {
            base.Load();
            LoadStyle.SetDifficulty(Difficulty.Easy);
        }
        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }
    }
}