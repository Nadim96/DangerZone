using System;
using Assets.Scripts.Scenario;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// Show info on the ingame UI.
    /// </summary>
    public class ScenarioInfoUI : MonoBehaviour
    {
        public Scenario.BunkerScenario Scenario;
        public GameObject DoelImmobilise, DoelNeutralise;
        public TextMeshProUGUI Alive, Dead, TimeDuration;
        public float UpdateInterval = 0.1f;
        private float _lastUpdate;

        // Use this for initialization
        private void Start()
        {
            _lastUpdate = 0;
            if (Scenario == null)
                throw new NullReferenceException("ScenarioInfoUI needs a reference to a Scenario.");

            switch (Scenario.GoalType)
            {
                case GoalType.Neutralise:
                    DoelNeutralise.SetActive(true);
                    break;
                case GoalType.Immobilise:
                    DoelImmobilise.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Update is called once per frame
        private void Update()
        {

            if (Time.time > _lastUpdate + UpdateInterval && Scenario.Started)
            {
                int aliveTargets = Scenario.AliveTargets;
                int deadTargets = Scenario.Targets.Count - aliveTargets;

                Alive.text = aliveTargets.ToString();
                Dead.text = deadTargets.ToString();

                float time = Time.time - Scenario.ScenarioStartedTime;
                TimeDuration.text = time.ToString("0.0s");
                _lastUpdate = Time.time;
            }
        }
    }
}