using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HitView
{
    /// <summary>
    /// Information on a hit to a npc
    /// </summary>
    public class HitInfo
    {
        /// <summary>
        /// Body part that is hit
        /// </summary>
        public GameObject BodyPart { get; private set; }

        /// <summary>
        /// HitLocation in localspace to bodypart
        /// </summary>
        public Vector3 HitLocation { get; private set; }

        public HitInfo(GameObject bodypart, Vector3 hitLocation)
        {
            this.BodyPart = bodypart;
            this.HitLocation = hitLocation;
        }


    }
}
