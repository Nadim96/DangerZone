using Assets.Scripts.NPCs;
using UnityEngine;

namespace Assets.Scripts.HitView
{
    /// <summary>
    /// Class for saving necessary info to use in HitView.
    /// </summary>
    public class HitLocation
    {
        private static ShowNpcHit _showNpcHit;

        private static ShowNpcHit ShowNpcHit
        {
            get
            {
                if (_showNpcHit == null)
                    _showNpcHit = Object.FindObjectOfType<ShowNpcHit>();
                return _showNpcHit;
            }
        }

        /// <summary>
        /// Save the SkinRig of the NPC in the current position
        /// </summary>
        /// <param name="npc">NPC gameobject to save</param>
        /// <returns>skinRig of NPC</returns>
        public static GameObject Save(NPC npc)
        {
            GameObject skinrig = npc.transform.Find("AnimRig").Find("SkinRig").gameObject;
            //only save the current skinrig to reduce memory usage
            GameObject npcRig = Object.Instantiate(skinrig);

            //set correct position
            npcRig.transform.parent = ShowNpcHit.NpcObject.transform;
            npcRig.transform.position = skinrig.transform.position;
            npcRig.transform.rotation = skinrig.transform.rotation;

            //add spotlight
            GameObject lightObject = Object.Instantiate(ShowNpcHit.Light);
            Light light = lightObject.GetComponent<Light>();

            //make this visible when selecting in UI
            //can specify more later with only highlighting gun drawn NPCS etc.

            if (npc.IsHostile)
            {
                light.color = Color.red;
            }
            else
            {
                light.color = Color.blue;
            }

            lightObject.transform.parent = npcRig.transform;
            lightObject.transform.localPosition = Vector3.up * 3;

            npcRig.SetActive(false);

            return npcRig;
        }

        /// <summary>
        /// Save the SkinRig of the NPC in the current position with point of hit
        /// </summary>
        /// <param name="npc">NPC gameobject to save</param>
        /// <param name="worldPoint">Point of the NPC that got Hit</param>
        /// <returns>SkinRig of NPC with red hitpoint</returns>
        public static GameObject Save(NPC npc, Vector3 worldPoint)
        {
            GameObject NPC = Save(npc);

            Vector3 localPoint = NPC.transform.InverseTransformPoint(worldPoint);

            // Create hitpoint and set it as child object to npc
            GameObject point = CreateHitPoint();
            point.transform.parent = NPC.transform;
            point.transform.localPosition = localPoint - new Vector3(0, 0.3f, 0);

            return NPC;
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