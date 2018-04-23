using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPro;

using Assets.Scripts.NPCs;
using Assets.Scripts.Utility;
using UnityEditor;

namespace Assets.Scripts.HitView
{
    /// <summary>
    /// Handling clicks in the shownpchitPanel
    /// </summary>
    public class ShowNpcHit : MonoBehaviour
    {
        /// <summary>
        /// The spawn location of the doll
        /// </summary>
        [SerializeField]
        private Transform SpawnLoaction;

        /// <summary>
        /// Current doll that is placed
        /// </summary>
        private Doll Current;

        /// <summary>
        /// Stores all of the hits per hit NPC.
        /// </summary>
        public SortedList<NPC, List<HitInfo>> HitNPCs { get; set; }

        /// <summary>
        /// Starts the behavior
        /// </summary>
        public void Start()
        {
            HitNPCs = new SortedList<NPC, List<HitInfo>>();
        }

        /// <summary>
        /// Saving an npc to location
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="hitlocation"></param>
        public void Save(NPC npc, HitMessage hitMessage)
        {
            Vector3 location = hitMessage.HitObject.transform.InverseTransformPoint(hitMessage.PointOfImpact);

            if (HitNPCs.Count == 0)
            {
                Create(npc);
            }

            if (HitNPCs.ContainsKey(npc))
            {
                HitNPCs[npc].Add(new HitInfo(hitMessage.HitObject, location));
            }
            else
            {
                HitNPCs.Add(npc, new List<HitInfo>() { new HitInfo(hitMessage.HitObject, location) });
            }
        }

        /// <summary>
        /// Set previous GameObject in the list active.
        /// </summary>
        public void Prev()
        {
            if (Current == null) return;

            int index = HitNPCs.IndexOfKey(Current.NPC);

            if (index > 0)
            {
                Delete();
                Create(HitNPCs.Keys[index - 1]);
            }
        }

        /// <summary>
        /// Set next GameObject in the list active.
        /// </summary>
        public void Next()
        {
            if (Current == null) return;

            int index = HitNPCs.IndexOfKey(Current.NPC);

            if (index < HitNPCs.Count - 1)
            {
                Delete();
                Create(HitNPCs.Keys[index + 1]);
            }
        }

        /// <summary>
        /// creates a "doll" for hitfeedback
        /// </summary>
        /// <param name="npc"></param>
        private void Create(NPC npc)
        {
            List<HitInfo> hits = HitNPCs[npc];

            GameObject skin = GameObject.Instantiate(PrefabUtility.FindPrefabRoot(npc.gameObject).transform.Find("AnimRig").Find("SkinRig").gameObject);

            foreach (HitInfo hit in hits)
            {
                GameObject hitpoint = CreateHitPoint();
                hitpoint.transform.SetParent(skin.transform.Find(hit.BodyPart.name));
                hitpoint.transform.position = hit.HitLocation;
            }

            Current = new Doll(npc, skin);
        }

        /// <summary>
        /// deletes a "doll" for hitfeedback
        /// </summary>
        /// <param name="npc"></param>
        private void Delete()
        {
            if (Current == null) return;
            GameObject.Destroy(Current.Skin);
            Current = null;
        }

        /// <summary>
        /// Gameobject that shows hit location
        /// </summary>
        /// <returns>hitlocation sphere</returns>
        private static GameObject CreateHitPoint()
        {
            GameObject hitObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitObject.transform.localScale *= 0.2f;
            hitObject.GetComponent<Renderer>().material.color = Color.magenta;
            hitObject.SetActive(true);
            return hitObject;
        }
    }
}