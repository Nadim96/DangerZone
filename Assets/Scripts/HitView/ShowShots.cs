using Assets.Scripts.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HitView
{
    public class ShowShots : MonoBehaviour
    {
        public Material shader;

        /// <summary>
        /// List of shot representation
        /// </summary>
        private List<GameObject> Shots = new List<GameObject>();

        /// <summary>
        /// Saving the shot to be shown
        /// </summary>
        /// <param name="shot"></param>
        public void Save(Shot shot)
        {
            Color color = (shot.Hit.GetComponentInParent<NPC>() != null) ? Color.green : Color.red;

            GameObject sr = CreateShotRepresentation(shot.Origin, shot.ImpactPoint, color);

            NPC npc = shot.Hit.GetComponentInParent<NPC>();
            if (npc != null)
            {
                GameObject skin = CreateHitSkin(npc.gameObject);
                 Shots.Add(skin);
            }

            Shots.Add(sr);
        }

        /// <summary>
        /// Resets and cleans all the shots
        /// </summary>
        public void Reset()
        {
            foreach (GameObject shot in Shots)
            {
                GameObject.Destroy(shot);
            }
            Shots.Clear();
        }

        /// <summary>
        /// Shows the shots
        /// </summary>
        /// <param name="show"></param>
        public void Show(bool show)
        {
            foreach (GameObject shot in Shots)
            {
                shot.SetActive(show);
            }
        }

        /// <summary>
        /// Creates the hit skin for feedback
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private GameObject CreateHitSkin(GameObject npc)
        {
            GameObject skin = GameObject.Instantiate(npc.gameObject.transform.Find("AnimRig").Find("SkinRig").gameObject);
            skin.transform.parent = transform;

            skin.transform.position = npc.transform.position;
            skin.transform.rotation = npc.transform.rotation;
            skin.transform.Rotate(Vector3.up, 180);
            skin.SetActive(false);

            SkinnedMeshRenderer[] smrs = skin.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                smr.materials = new Material[] { shader , shader , shader };
            }
            return skin;
        }

        /// <summary>
        /// Creates the shot representation from shot info
        /// </summary>
        /// <param name="start">position where the shot started</param>
        /// <param name="end">position where the shot ended</param>
        /// <param name="color">color showing whether its hit or not</param>
        /// <returns></returns>
        private GameObject CreateShotRepresentation(Vector3 start, Vector3 end, Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            float thickness = 0.005f;
            float length = Vector3.Distance(start, end);

            obj.transform.localScale = new Vector3(thickness, thickness, length);
            obj.transform.position = start + ((end-start)/2);
            obj.transform.parent = transform;
            obj.transform.LookAt(end);

            obj.GetComponent<MeshRenderer>().material.color = color;

            obj.SetActive(false);

            return obj;
        }
    }
}
