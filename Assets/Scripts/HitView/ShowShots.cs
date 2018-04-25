using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.HitView
{
    public class ShowShots : MonoBehaviour
    {
        private List<Shot> ShotsFired = new List<Shot>();

        public void Save(Shot shot)
        {
            ShotsFired.Add(shot);
        }

        public void Update()
        {
            
        }

        public void Reset()
        {
            ShotsFired.Clear();
        }
    }
}
