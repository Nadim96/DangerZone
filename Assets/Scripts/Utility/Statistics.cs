using Assets.Scripts.HitView;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// A class that saves all statistics in a scene and prints them to the UI 
    /// </summary>
    public class Statistics : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _gameTime;
        [SerializeField] private TextMeshProUGUI _panicTime;
        [SerializeField] private TextMeshProUGUI _lofCivilians;
        [SerializeField] private TextMeshProUGUI _lofWrongEnemies;
        [SerializeField] private TextMeshProUGUI _lofTime;
        [SerializeField] private TextMeshProUGUI _shotsFired;
        [SerializeField] private TextMeshProUGUI _friendliesInScene;
        [SerializeField] private TextMeshProUGUI _enemiesInScene;
        [SerializeField] private TextMeshProUGUI _deadEnemies;
        [SerializeField] private TextMeshProUGUI _deadFriendlies;
        [SerializeField] private TextMeshProUGUI _timesHit;

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
        /// Timestamp Panic Started.
        /// </summary>
        /// <remarks>PanicStarted can be initiated with Time.time</remarks>
        public static float PanicStarted { get; set; }

        /// <summary>
        /// time since last update
        /// </summary>
        /// <remarks>is used to make sure an update is only fired once a second</remarks>
        private float _lastUpdate;

        void Start()
        {
            TimeSpentAimingOnCivilians = 0;
            TimeSpentAimingOnHostiles = 0;
            DeadFriendliesByEnemy = 0;
            DeadFriendliesByPlayer = 0;
            DeadHostilesByEnemy = 0;
            DeadHostilesByPlayer = 0;
            PlayerHit = 0;
            ShotsFired = 0;

            _lastUpdate = Time.time;
        }

        void Update()
        {
            //update only once every second
            if (_lastUpdate + 1 > Time.time) return;
            _lastUpdate = Time.time;


            _gameTime.SetText(FormatTime(Time.timeSinceLevelLoad));

            // Percentage aimed at hostiles
            int percentage = (int) Math.Round(TimeSpentAimingOnHostiles / (TimeSpentAimingOnCivilians > 0 ? TimeSpentAimingOnCivilians : 1) * 100);

            _lofCivilians.SetText("Tijd op burgers gericht: " + string.Format("{0}s", Math.Round(TimeSpentAimingOnCivilians)));
            _lofWrongEnemies.SetText("Tijd op verdachten gericht: " + string.Format("{0}s", Math.Round(TimeSpentAimingOnHostiles, 2)));
            _lofTime.SetText("Speler richtte {0}% van de tijd op verdachten", percentage);

            _shotsFired.SetText("Schoten: " + ShotsFired.ToString());

            _deadFriendlies.SetText("Dode burgers: " + string.Format("{0}", DeadFriendliesByEnemy + DeadFriendliesByPlayer));
            _deadEnemies.SetText("Dode verdachten: " + string.Format("{0}", DeadHostilesByEnemy + DeadHostilesByPlayer));

            _timesHit.SetText("Keren geraakt:" + PlayerHit.ToString());
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
        /// Reset Statistic parameters
        /// </summary>
        public static void Reset()
        {
            TimeSpentAimingOnCivilians = 0;
            TimeSpentAimingOnHostiles = 0;
            DeadFriendliesByEnemy = 0;
            DeadFriendliesByPlayer = 0;
            DeadHostilesByEnemy = 0;
            DeadHostilesByPlayer = 0;
            PlayerHit = 0;

            ShowNpcHit showNpcHit = UnityEngine.Object.FindObjectOfType<ShowNpcHit>();
            if (showNpcHit != null)
            {
                showNpcHit.Reset();
            }

        }
    }
}