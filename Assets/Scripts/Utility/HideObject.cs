using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Utility
{
    public class HideObject : MonoBehaviour
    {
        public static List<HideObject> HideObjects = new List<HideObject>();
        /// <summary>
        /// max NPCs that can hide at this object
        /// </summary>
        [SerializeField]
        private int MaxNPCs = 1;

        private int currentNPCs;
        public void Start()
        {
            HideObjects.Add(this);
        }

        public Vector3 HidePos(Vector3 hostilePos)
        {
            currentNPCs++;
            if (currentNPCs == MaxNPCs)
            {
                //remove from list to prevent hiding of more people
                HideObjects.Remove(this);
            }

            //direction ab = b-a
            //get direction from hostile to hideObject
            Vector3 dir = transform.position - hostilePos;
            dir.Normalize();

            NavMeshHit hit;
            float i = 0.5f;
            //loop until a good point to place is found
            while (true)
            {
                if (!NavMesh.SamplePosition(transform.position + (dir * i), out hit, .5f, NavMesh.AllAreas))
                {
                    i += 0.2f;
                }
                else
                {
                    break;
                }

            }
            return hit.position;
        }
    }
}