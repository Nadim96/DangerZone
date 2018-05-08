using Assets.Scripts.HitView;
using Assets.Scripts.NPCs;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// A class that saves all statistics in a scene and prints them to the UI 
    /// </summary>
    public class Statistics : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timeAimedOnCivilians;
        [SerializeField] private TextMeshProUGUI _shotsFired;

        //Other feedback objects
        public ShowShots showShots;

        /// <summary>
        /// Amount of time in seconds player spent aiming at civilians  
        /// </summary>
        public static float TimeSpentAimingOnCivilians { get; set; }

        /// <summary>
        /// Amount of time in seconds player spent aiming at hostiles  
        /// </summary>
        public static float TimeSpentAimingOnHostiles { get; set; }

        /// <summary>
        /// Number of friendly NPCs killed by Enemies
        /// </summary>
        public static int DeadFriendliesByEnemy { get; set; }

        /// <summary>
        /// Amount of fFriendly NPCs ingame killed by player
        /// </summary>
        public static int DeadFriendliesByPlayer { get; set; }

        /// <summary>
        /// Amount of hostile NPCs killed by Enemies
        /// </summary>
        public static int DeadHostilesByEnemy { get; set; }

        /// <summary>
        /// Amount of hostile NPCs killed by player
        /// </summary>
        public static int DeadHostilesByPlayer { get; set; }

        /// <summary>
        /// Amount of times player got hit
        /// </summary>
        public static int PlayerHit { get; set; }

        /// <summary>
        /// Amount of times the player shot 
        /// </summary>
        public static int ShotsFired { get; set; }

        /// <summary>
        /// Amount of times  the player hit an npc
        /// </summary>
        public static int ShotsHit { get; set; }

        /// <summary>
        /// Timestamp Panic Started.
        /// </summary>
        /// <remarks>PanicStarted can be initiated with Time.time</remarks>
        public static float PanicStarted { get; set; }

        /// <summary>
        /// List of npc's who have been aimed at
        /// </summary>
        public static List<NPC> NpcsAimedAt { get; set; }

        /// <summary>
        /// time since last update
        /// </summary>
        /// <remarks>is used to make sure an update is only fired once a second</remarks>
        private float _lastUpdate;

        void Start()
        {
            NpcsAimedAt = new List<NPC>();
            TimeSpentAimingOnCivilians = 0;
            TimeSpentAimingOnHostiles = 0;
            DeadFriendliesByEnemy = 0;
            DeadFriendliesByPlayer = 0;
            DeadHostilesByEnemy = 0;
            DeadHostilesByPlayer = 0;
            PlayerHit = 0;
            ShotsFired = 0;
            ShotsHit = 0;

            _lastUpdate = Time.time;
        }

        void Update()
        {
            //update only once every second
            if (_lastUpdate + 1 > Time.time) return;
            _lastUpdate = Time.time;

            //_gameTime.SetText(FormatTime(Time.timeSinceLevelLoad));

            //// Percentage aimed at hostiles
            //int percentage = (int) Math.Round(TimeSpentAimingOnHostiles / (TimeSpentAimingOnCivilians > 0 ? TimeSpentAimingOnCivilians : 1) * 100);

            //_lofWrongEnemies.SetText("Tijd op verdachten gericht: " + string.Format("{0}s", Math.Round(TimeSpentAimingOnHostiles, 2)));
            //_lofTime.SetText("Speler richtte {0}% van de tijd op verdachten", percentage);
            //_deadFriendlies.SetText("Dode burgers: " + string.Format("{0}", DeadFriendliesByEnemy + DeadFriendliesByPlayer));
            //_deadEnemies.SetText("Dode verdachten: " + string.Format("{0}", DeadHostilesByEnemy + DeadHostilesByPlayer));
            //_timesHit.SetText("Keren geraakt:" + PlayerHit.ToString());


            int hitPercentage = ShotsFired > 0 ? (int)((ShotsHit*1.0) / (ShotsFired*1.0) * 100.0) : 0;

            _shotsFired.SetText("Schoten raak: " + ShotsHit + "/" + ShotsFired + "(" + hitPercentage+ "%)");

            int civsAimedAt = NpcsAimedAt.FindAll(t => t.IsHostile == false).Count;

            _timeAimedOnCivilians.SetText("Burgers aangewezen: " + civsAimedAt + string.Format("({0}s)", Math.Round(TimeSpentAimingOnCivilians,2)));
        }

        /// <summary>
        /// format time fom seconds to minute
        /// </summary>
        /// <param name="time">Time in seconds</param>
        /// <returns>Time in minutes</returns>
        private string FormatTime(float time)
        {
            return string.Format("{0}m; {1}{2}s", Math.Floor(time / 60), time % 60 < 10 ? "0" : "",
                Math.Floor(time % 60));
        }

        /// <summary>
        /// Enables the statistics and feedback
        /// </summary>
        /// <param name="show"></param>
        public static void Show(bool show)
        {
            Statistics stats = UnityEngine.Object.FindObjectOfType<Statistics>();
            if (stats != null)
            {
                stats.showShots.Show(show);
            }
        }

        /// <summary>
        /// Reset Statistic parameters
        /// </summary>
        public static void Reset()
        {
            NpcsAimedAt.Clear();
            TimeSpentAimingOnCivilians = 0;
            TimeSpentAimingOnHostiles = 0;
            DeadFriendliesByEnemy = 0;
            DeadFriendliesByPlayer = 0;
            DeadHostilesByEnemy = 0;
            DeadHostilesByPlayer = 0;
            PlayerHit = 0;
            ShotsFired = 0;
            ShotsHit = 0;

          
        }
    }
}