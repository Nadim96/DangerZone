using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HitView
{
    public class Shot
    {
        /// <summary>
        /// Wether the shot hit a npc
        /// </summary>
        public GameObject Hit { get; private set; }

        /// <summary>
        /// Origin of the shot
        /// </summary>
        public Vector3 Origin { get; private set; }

        /// <summary>
        /// Origin of the shot
        /// </summary>
        public GameObject OriginObject { get; private set; }

        /// <summary>
        /// Impact point of the shot
        /// </summary>
        public Vector3 ImpactPoint { get; private set; }

        public Shot(GameObject hit, Vector3 origin, GameObject originObject, Vector3 impactPoint)
        {
            this.Hit = hit;
            this.Origin = origin;
            this.ImpactPoint = impactPoint;
            this.OriginObject = originObject;
        }
    }
}
