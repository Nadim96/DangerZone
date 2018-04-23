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
        public Dictionary<NPC, List<HitInfo>> HitNPCs { get; set; }

        /// <summary>
        /// Starts the behavior
        /// </summary>
        public void Start()
        {
            HitNPCs = new Dictionary<NPC, List<HitInfo>>();
        }

        /// <summary>
        /// Saving an npc to location
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="hitlocation"></param>
        public void Save(NPC npc, HitMessage hitMessage)
        {
            Vector3 location = hitMessage.HitObject.transform.InverseTransformPoint(hitMessage.PointOfImpact);

            Debug.Log(hitMessage.PointOfImpact + " " + location);

            int count = HitNPCs.Count;

            if (HitNPCs.ContainsKey(npc))
            {
                HitNPCs[npc].Add(new HitInfo(hitMessage.HitObject, location));
            }
            else
            {
                HitNPCs.Add(npc, new List<HitInfo>() { new HitInfo(hitMessage.HitObject, location) });
            }

            if (count == 0)
            {
                Create(npc);
            }
            else
            {
                if (Current.NPC == npc)
                {
                    ReCreate(npc);
                }
            }
        }

        /// <summary>
        /// Set previous GameObject in the list active.
        /// </summary>
        public void Prev()
        {
            if (Current == null) return;

            int index = GetIndexForKey(Current.NPC);

            if (index > 0)
            {
                ReCreate(GetKeyForIndex(index - 1));
            }
        }

        /// <summary>
        /// Set next GameObject in the list active.
        /// </summary>
        public void Next()
        {
            if (Current == null) return;

            int index = GetIndexForKey(Current.NPC);

            if (index < HitNPCs.Count - 1)
            {
                ReCreate(GetKeyForIndex(index + 1));
            }
        }

        /// <summary>
        /// Recreates the npc
        /// </summary>
        /// <param name="npc"></param>
        private void ReCreate(NPC npc)
        {
            Delete();
            Create(npc);
        }

        /// <summary>
        /// creates a "doll" for hitfeedback
        /// </summary>
        /// <param name="npc"></param>
        private void Create(NPC npc)
        {
            List<HitInfo> hits = HitNPCs[npc];

            //GameObject skin = GameObject.Instantiate(npc.gameObject.transform.Find("AnimRig").Find("SkinRig").gameObject);
            GameObject skin = GameObject.Instantiate(npc.gameObject.transform.Find("AnimRig").Find("SkinRig").gameObject);

            // PrefabUtility.ResetToPrefabState(skin);

            PrefabUtility.RevertPrefabInstance(skin);

            skin.transform.SetParent(SpawnLoaction);
            skin.transform.localPosition = new Vector3(0, 0, 0);

            foreach (HitInfo hit in hits)
            {
                GameObject hitpoint = CreateHitPoint();
                Transform bodypart = FindDeepChild(skin.transform, hit.BodyPart.name.Trim());
                hitpoint.transform.SetParent(bodypart);
                hitpoint.transform.localPosition = hit.HitLocation;
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
            hitObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            hitObject.GetComponent<Renderer>().material.color = Color.magenta;
            hitObject.SetActive(true);
            return hitObject;
        }

        /// <summary>
        /// Gets index for npc key
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private int GetIndexForKey(NPC npc)
        {
            int index = 0;

            foreach (NPC found in HitNPCs.Keys)
            {
                if (npc == found) break;
                index++;
            }

            return index;
        }

        /// <summary>
        /// Gets key for index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private NPC GetKeyForIndex(int index)
        {
            int count = 0;

            foreach (NPC found in HitNPCs.Keys)
            {
                if (count == index) return found;
                count++;
            }

            return null;
        }

        public static Transform FindDeepChild(Transform aParent, string aName)
        {
            Transform[] childeren = aParent.GetComponentsInChildren<Transform>();

            foreach (Transform child in childeren)
            {
                if (child.name.Equals(aName)) return child;
            }

            return null;
        }

    }
}