using Assets.Scripts.BehaviourTree;
using UnityEngine;
namespace Assets.Scripts.Scenario
{
    public class StreetScenario : ScenarioBase
    {
        protected override void Load()
        {
            LoadStyle.SetDifficulty(Difficulty.Street);
            base.Load();
     
        }
        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }
    }
}