using System.Collections.Generic;
using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// base class of target used save and load positions of NPCs
    /// </summary>
    public abstract class Target
    {
        public Vector3 Position
        {
            get { return Waypoints[0].Position; }
            set
            {
                if (Waypoints.Count == 0)
                    Waypoints.Add(ScenarioBase.Instance.CreateWaypoint(this,value));
                else
                    Waypoints[0].Position = value;
            }
        }

        public Quaternion Rotation
        {
            get { return Waypoints[0].transform.rotation; }
            set {
                Waypoints[0].transform.rotation = value;
            }
        }
        public abstract bool IsAlive { get; }
        public bool IsHostile { get; set; }
        public ItemType ItemType { get; set; }
        public Difficulty Difficulty { get; set; }
        public List<Waypoint> Waypoints { get; private set; }

        protected Target()
        {
            Waypoints = new List<Waypoint>();
        }

        public abstract Transform Spawn(Transform prefab);

        public abstract void Destroy();
    }
}
