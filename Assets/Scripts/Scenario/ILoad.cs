using Assets.Scripts.BehaviourTree;

namespace Assets.Scripts.Scenario
{
    public interface ILoad
    {
        void SetDifficulty(Difficulty difficulty);
        void Load();
    }
}