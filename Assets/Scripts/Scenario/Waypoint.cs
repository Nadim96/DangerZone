using UnityEngine;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// Waypoints are created in the editor mode by the player or when loading a scenario.
    /// </summary>
    public class Waypoint : MonoBehaviour
    {
        public Target Owner { get; set; }

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        
    }
}