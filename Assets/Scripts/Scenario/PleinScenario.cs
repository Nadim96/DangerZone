using Assets.Scripts.BehaviourTree;
using UnityEngine;
namespace Assets.Scripts.Scenario
{
    public class PleinScenario : ScenarioBase
    {
        protected override void Load()
        {
            base.Load();
            LoadStyle.SetDifficulty(Difficulty.Easy);
         
            LoadRandom random = (LoadRandom) LoadStyle;
            random.Plein = true;
        }

        public override void Stop()
        {
            BehaviourTree.Leaf.Actions.CausePanic._isTriggered = false;
            base.Stop();
        }
    }
}