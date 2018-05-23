using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Environment;
using Assets.Scripts.Scenario;

namespace Assets.Scripts.Settings
{
    /// <summary>
    /// Static class storing all parameters of scenario's. 
    /// Is needed to correctly start scenario's.
    /// </summary>
    public static class ScenarioSettings
    {
        #region Defaults

        private const int DEFAULT_NUM_OF_PEOPLE = 20;
        private const TimeOfDayType DEFAULT_TIME_OF_DAY = TimeOfDayType.Noon;
        private const WeatherType DEFAULT_WEATHER = WeatherType.Sunny;

        private const bool DEFAULT_IS_RANDOM_SCENARIO = true;

        private const int DEFAULT_MIN_ENEMIES = 1;
        private const int DEFAULT_MAX_ENEMIES = 3;
        private const int DEFAULT_MIN_FRIENDLIES = 1;
        private const int DEFAULT_MAX_FRIENDLIES = 2;

        private const int DEFAULT_MIN_ROOMSIZE = 3;
        private const int DEFAULT_MAX_ROOMSIZE = 10;

        private const float DEFAULT_REACTION_TIME = 0.5f;
        private const bool DEFAULT_LIGHTS = true;

        private const LevelType DEFAULT_LEVEL_TYPE = LevelType.Middle;
        private const TargetType DEFAULT_TARGET_TYPE = TargetType.Person;
        private const GoalType DEFAULT_GOAL_TYPE = GoalType.Neutralise;
        private const WeaponSize DEFAULT_WEAPON_SIZE = WeaponSize.Small;
        private const MovementType DEFAULT_MOVEMENT_TYPE = MovementType.Still;

        private const int DEFAULT_ENGAGEMENT_DISTANCE = 10;

        #endregion

        public static int NumOfPeople { get; set; }
        public static TimeOfDayType TimeOfDay { get; set; }
        public static WeatherType Weather { get; set; }

        public static bool IsRandomScenario { get; set; }

        public static int MinEnemies { get; set; }
        public static int MaxEnemies { get; set; }
        public static int MinFriendlies { get; set; }
        public static int MaxFriendlies { get; set; }

        public static bool Lights { get; set; }

        public static int MinRoomSize { get; set; }
        public static int MaxRoomSize { get; set; }
        public static float ReactionTime { get; set; }

        public static LevelType LevelType { get; set; }
        public static TargetType TargetType { get; set; }
        public static GoalType GoalType { get; set; }
        public static WeaponSize WeaponSize { get; set; }
        public static MovementType MovementType { get; set; }

        public static int EngagementDistance { get; set; }

        static ScenarioSettings()
        {
            NumOfPeople = DEFAULT_NUM_OF_PEOPLE;
            TimeOfDay = DEFAULT_TIME_OF_DAY;
            Weather = DEFAULT_WEATHER;

            IsRandomScenario = DEFAULT_IS_RANDOM_SCENARIO;
            MinEnemies = DEFAULT_MIN_ENEMIES;
            MaxEnemies = DEFAULT_MAX_ENEMIES;
            MinFriendlies = DEFAULT_MIN_FRIENDLIES;
            MaxFriendlies = DEFAULT_MAX_FRIENDLIES;

            MinRoomSize = DEFAULT_MIN_ROOMSIZE;
            MaxRoomSize = DEFAULT_MAX_ROOMSIZE;
            ReactionTime = DEFAULT_REACTION_TIME;
            LevelType = DEFAULT_LEVEL_TYPE;
            TargetType = DEFAULT_TARGET_TYPE;
            GoalType = DEFAULT_GOAL_TYPE;
            WeaponSize = DEFAULT_WEAPON_SIZE;
            MovementType = DEFAULT_MOVEMENT_TYPE;

            Lights = true;

            EngagementDistance = DEFAULT_ENGAGEMENT_DISTANCE;
        }
    }
}