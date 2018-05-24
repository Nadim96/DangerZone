using Assets.Scripts.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Player;
using TMPro;

namespace Assets.Scripts.HitView
{
    public class ShowShots : MonoBehaviour
    {
        public Material WhiteShader;
        public Material OrangeShader;
        public Material RedShader;

        public Transform EnemyFire;
        public Transform FriendlyFire;

        public TextMeshProUGUI FeedbackText;

        public GameObject[] TipObjects;

        /// <summary>
        /// List of shot representation
        /// </summary>
        private FeedbackCollection FriendlyShots = new FeedbackCollection("EIGEN VUUR");
        private FeedbackCollection EnemyShots = new FeedbackCollection("VIJANDELIJKE VUUR");
        private FeedbackCollection Tips = new FeedbackCollection("TIPS");

        public List<FeedbackCollection> AllFeedback;
        public int current = 0;

        public void Start()
        {
            Tips.Objects.AddRange(TipObjects);
            if (Settings.ScenarioSettings.IsBeginnerNiveau)
            {
                AllFeedback = new List<FeedbackCollection>() { FriendlyShots, EnemyShots, Tips };
            }
            else {
                AllFeedback = new List<FeedbackCollection>() { FriendlyShots, EnemyShots};
            }
        }

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
                FriendlyShots.Objects.Add(sr);

                if (npc != null)
                {
                    Material m = npc.IsHostile ? WhiteShader : OrangeShader;
                    GameObject skin = CreateHitSkin(npc.gameObject, m, FriendlyFire);
                    FriendlyShots.Objects.Add(skin);
                }
            }
            else
            {
                GameObject sr = CreateShotRepresentation(shot.Origin, shot.ImpactPoint, Color.red, EnemyFire);
                NPC npc = shot.OriginObject.GetComponentInParent<NPC>();
                GameObject skinenemy = CreateHitSkin(npc.gameObject, WhiteShader, FriendlyFire);
                EnemyShots.Objects.Add(sr);
                EnemyShots.Objects.Add(skinenemy);

                Player.Player player = shot.Hit.GetComponentInParent<Player.Player>();
                if (player != null)
                {
                    GameObject sphere = CreateHitSphere(shot.ImpactPoint, EnemyFire);
                    EnemyShots.Objects.Add(sphere);

                    if (!player.IsAlive)
                    {
                        GameObject skin = CreateHitSkin(shot.OriginObject, RedShader, EnemyFire);
                        EnemyShots.Objects.Add(skin);
                    }
                }
            }
        }

        /// <summary>
        /// Resets and cleans all the shots
        /// </summary>
        public void Reset()
        {
            current = 0;
            FriendlyShots.Clear();
            EnemyShots.Clear();
        }

        /// <summary>
        /// Shows the shots
        /// </summary>
        /// <param name="show"></param>
        public void Show(bool show)
        {
            foreach (FeedbackCollection fc in AllFeedback)
            {
                fc.Show(false);
            }
            AllFeedback[current].Show(show);
            SetText();
        }

        /// <summary>
        /// Goes to the next feedback
        /// </summary>
        public void Next()
        {
            if (current == AllFeedback.Count - 1) 
                current = 0;
             else current++;
          
            Show(true);
        }

        /// <summary>
        /// Goes to the previous feedback
        /// </summary>
        public void Previous()
        {
            if (current == 0) current = AllFeedback.Count;
            else  current--;
            Show(true);
        }

        /// <summary>
        /// sets the correct feedback text
        /// </summary>
        public void SetText()
        {
            FeedbackText.text = AllFeedback[current].Text;
        }

        /// <summary>
        /// Creates the hit skin for feedback
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private GameObject CreateHitSkin(GameObject npc, Material shader, Transform parent)
        {
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
            sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
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
            obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            obj.SetActive(false);

            return obj;
        }
    }
}
