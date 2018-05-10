using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HitView
{
    /// <summary>
    /// Collection of feedback
    /// </summary>
    public class FeedbackCollection
    {
        /// <summary>
        /// List of objects that give feedback
        /// </summary>
        public List<GameObject> Objects { get; private set; }

        /// <summary>
        /// Text describing the feedback
        /// </summary>
        public string Text { get; private set; }

        public FeedbackCollection(string text)
        {
            this.Objects = new List<GameObject>();
            this.Text = text;
        }

        /// <summary>
        /// shows the feedback
        /// </summary>
        /// <param name="show"></param>
        public void Show(bool show)
        {
            foreach (GameObject obj in Objects)
            {
                obj.SetActive(show);
            }
        }

        /// <summary>
        /// Clears the feedback
        /// </summary>
        public void Clear()
        {
            foreach (GameObject shot in Objects)
            {
                GameObject.Destroy(shot);
            }
            Objects.Clear();
        }

    }
}
