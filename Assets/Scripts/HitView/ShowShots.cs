using Assets.Scripts.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.HitView
{
    public class ShowShots : MonoBehaviour
    {
        public Material WhiteShader;
        public Material OrangeShader;
        public Material RedShader;

        public Transform EnemyFire;
        public Transform FriendlyFire;

        /// <summary>
        /// List of shot representation
        /// </summary>
        private List<GameObject> FriendlyShots = new List<GameObject>();
        private List<GameObject> EnemyShots = new List<GameObject>();

        /// <summary>
        /// Saving the shot to be shown
        /// </summary>
        /// <param name="shot"></param>
        public void Save(Shot shot, bool friendly)
        {
            if (friendly)
            {
                NPC npc = shot.Hit.GetComponentInParent<NPC>();
                Color color = (npc != null) ? (npc.IsHostile ? Color.green : new Color(1f, 0.8f, 0f, 1f)) : Color.red;

                GameObject sr = CreateShotRepresentation(shot.Origin, shot.ImpactPoint, color, FriendlyFire);
                FriendlyShots.Add(sr);

                if (npc != null)
                {
                    Material m = npc.IsHostile ? WhiteShader : OrangeShader;
                    GameObject skin = CreateHitSkin(npc.gameObject, m, FriendlyFire);
                    FriendlyShots.Add(skin);
                }
            }
            else
            {
                GameObject sr = CreateShotRepresentation(shot.Origin, shot.ImpactPoint, Color.red, EnemyFire);
                EnemyShots.Add(sr);

                Player.Player player = shot.Hit.GetComponentInParent<Player.Player>();
                if (player != null)
                {
                    GameObject sphere = CreateHitSphere(shot.ImpactPoint, EnemyFire);
                    EnemyShots.Add(sphere);

                    if (!player.IsAlive)
                    {
                        GameObject skin = CreateHitSkin(shot.OriginObject, RedShader, EnemyFire);
                        EnemyShots.Add(skin);
                    }
                }
            }
        }

        /// <summary>
        /// Resets and cleans all the shots
        /// </summary>
        public void Reset()
        {
            foreach (GameObject shot in FriendlyShots)
            {
                GameObject.Destroy(shot);
            }
            FriendlyShots.Clear();

            foreach (GameObject shot in EnemyShots)
            {
                GameObject.Destroy(shot);
            }
            EnemyShots.Clear();
        }

        /// <summary>
        /// Shows the shots
        /// </summary>
        /// <param name="show"></param>
        public void Show(bool show)
        {
            foreach (GameObject shot in FriendlyShots)
            {
                shot.SetActive(false);
            }
            foreach (GameObject shot in EnemyShots)
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
        private GameObject CreateHitSkin(GameObject npc, Material shader, Transform parent)
        {
            Debug.Log(npc.name);
            GameObject skin = GameObject.Instantiate(npc.gameObject.transform.Find("AnimRig").Find("SkinRig").gameObject);
            skin.transform.parent = parent;

            skin.transform.position = npc.transform.position;
            skin.transform.rotation = npc.transform.rotation;
            skin.transform.Rotate(Vector3.up, 180);
            skin.SetActive(false);

            SkinnedMeshRenderer[] smrs = skin.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                smr.materials = new Material[] { shader, shader, shader };
            }
            return skin;
        }

        /// <summary>
        /// Creates a small sphere to show a place that has been hit
        /// </summary>
        /// <returns></returns>
        private GameObject CreateHitSphere(Vector3 location, Transform parent)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            sphere.GetComponent<MeshRenderer>().material.color = Color.red;
            sphere.transform.parent = parent;
            sphere.transform.position = location;
            sphere.SetActive(false);

            return sphere;
        }

        /// <summary>
        /// Creates the shot representation from shot info
        /// </summary>
        /// <param name="start">position where the shot started</param>
        /// <param name="end">position where the shot ended</param>
        /// <param name="color">color showing whether its hit or not</param>
        /// <returns></returns>
        private GameObject CreateShotRepresentation(Vector3 start, Vector3 end, Color color, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            float thickness = 0.005f;
            float length = Vector3.Distance(start, end);

            obj.transform.localScale = new Vector3(thickness, thickness, length);
            obj.transform.position = start + ((end - start) / 2);
            obj.transform.parent = parent;
            obj.transform.LookAt(end);

            obj.GetComponent<MeshRenderer>().material.color = color;

            obj.SetActive(false);

            return obj;
        }
    }
}
