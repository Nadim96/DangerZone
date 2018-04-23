using Assets.Scripts.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HitView
{
    /// <summary>
    /// Information on the doll being shown in the ShowNpcHit.cs
    /// </summary>
    public class Doll
    {
        /// <summary>
        /// The NPC that the doll is made of
        /// </summary>
        public NPC NPC { get; internal set; }

        /// <summary>
        /// The skin of the doll to be shown
        /// </summary>
        public GameObject Skin { get; internal set; }

        public Doll(NPC npc, GameObject skin)
        {
            this.NPC = npc;
            this.Skin = skin;
        }
    }
}
