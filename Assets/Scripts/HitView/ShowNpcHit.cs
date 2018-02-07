using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.HitView
{
    /// <summary>
    /// Handling clicks in the shownpchitPanel
    /// </summary>
    public class ShowNpcHit : MonoBehaviour
    {
        /// <summary>
        /// Parent object of copied NPCs.
        /// </summary>
        public GameObject NpcObject;

        public GameObject Light;

        [SerializeField] private TextMeshProUGUI _currentShot;
        [SerializeField] private TextMeshProUGUI _totalShots;

        /// <summary>
        /// Stores all of the hits per hit NPC.
        /// </summary>
        public static List<List<GameObject>> HitNPCs { get; set; }

        private int _current;

        static ShowNpcHit()
        {
            HitNPCs = new List<List<GameObject>>();
        }

        /// <summary>
        /// set position of gameobjects in list
        /// </summary>
        public void InitializeList()
        {
            //set position of NPC's
            _currentShot.SetText((_current + 1).ToString());
            _totalShots.SetText(HitNPCs.Count.ToString());

            //spawn 0
            SetActive(_current, true);
        }

        public void DisableList()
        {
            SetActive(_current, false);
        }

        /// <summary>
        /// Set previous GameObject in the list active.
        /// </summary>
        public void Prev()
        {
            if (_current <= 0) return;
            SetActive(_current, false);
            _current--;
            SetActive(_current, true);
            _currentShot.SetText((_current + 1).ToString());
        }

        /// <summary>
        /// Set next GameObject in the list active.
        /// </summary>
        public void Next()
        {
            if (_current >= HitNPCs.Count - 1) return;
            SetActive(_current, false);
            _current++;
            SetActive(_current, true);
            _currentShot.SetText((_current + 1).ToString());
        }

        /// <summary>
        /// Set active state of GameObjects.
        /// </summary>
        /// <param name="current">Index of hitlist to be changed</param>
        /// <param name="active">Value to be set</param>
        private static void SetActive(int current, bool active)
        {
            if (current < 0 || current >= HitNPCs.Count)
                throw new IndexOutOfRangeException("Cannot get Hitlist at index " + current);

            HitNPCs[current].ForEach(x => x.SetActive(active));
        }
    }
}