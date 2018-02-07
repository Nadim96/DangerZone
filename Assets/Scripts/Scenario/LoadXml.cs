using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    public class LoadXml : ILoad
    {
        private const TargetType DEFAULT_TARGET_TYPE = TargetType.Person;
        private const GoalType DEFAULT_GOAL_TYPE = GoalType.Neutralise;
        private const string DEFAULT_SCENARIO = "Scenario 1.xml";

        /// <summary>
        /// Stores the filepath which will be used to load the correct scenario.
        /// This should be set by the main menu. Empty means this is a new scenario.
        /// </summary>
        public static string ScenarioToLoad { get; set; }

        private Difficulty difficulty;
        private ScenarioBase _scenario;

        public void Load()
        {
            _scenario = ScenarioBase.Instance;

            if (string.IsNullOrEmpty(ScenarioToLoad))
                ScenarioToLoad = string.Format("{0}/Scenarios/{1}", Application.streamingAssetsPath, DEFAULT_SCENARIO);

            LoadScenario(ScenarioToLoad);
        }

        private void LoadScenario(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Unable to load scenario. File not found.", path);

            XDocument xDocument = XDocument.Load(path);

            if (xDocument.Root == null)
                throw new NullReferenceException("Unable to load scenario. Root of XML file is empty.");

            if (_scenario == null)
                throw new NullReferenceException(
                    "Unable to load scenario. Ensure LoadLevel has a reference to a Scenario script.");

            LoadNameAndSettings(xDocument);

            LoadTargets(xDocument);
        }

        /// <summary>
        /// Loads and sets the scenario name and settings.
        /// </summary>
        /// <param name="xDocument"></param>
        private void LoadNameAndSettings(XDocument xDocument)
        {
            if (xDocument == null)
                throw new ArgumentNullException("xDocument");

            if (xDocument.Root == null)
                throw new NullReferenceException("Unable to load scenario. Root of XML file is empty.");

            XElement xName = xDocument.Root.Element("Name");
            if (xName != null && !xName.IsEmpty)
            {
                _scenario.Name = xName.Value;
            }
            else
            {
                _scenario.Name = "Unknown Scenario";
                Debug.LogWarning("Unable to find name of scenario.");
            }

            TargetType targetType = DEFAULT_TARGET_TYPE;
            GoalType goalType = DEFAULT_GOAL_TYPE;

            XElement xSettings = xDocument.Root.Element("Settings");
            if (xSettings != null && !xSettings.IsEmpty)
            {
                //get target type
                XElement xTarget = xSettings.Element("TargetType");
                if (xTarget != null && !xTarget.IsEmpty && Enum.IsDefined(typeof(TargetType), xTarget.Value))
                {
                    targetType = (TargetType) Enum.Parse(typeof(TargetType), xTarget.Value);
                }
                else Debug.LogWarning("Unable to determine Target type. Reverting to defaults.");

                //get goal type
                XElement xGoal = xSettings.Element("GoalType");
                if (xGoal != null && !xGoal.IsEmpty && Enum.IsDefined(typeof(GoalType), xGoal.Value))
                {
                    goalType = (GoalType) Enum.Parse(typeof(GoalType), xGoal.Value);
                }
                else Debug.LogWarning("Unable to determine Goal type. Reverting to defaults.");
            }
            else
            {
                Debug.LogWarning("Unable to find settings. Setting defaults.");
            }

            _scenario.TargetType = targetType;
            _scenario.GoalType = goalType;
        }

        /// <summary>
        /// Loads all targets and adds them to the scenario.
        /// </summary>
        /// <param name="xDocument"></param>
        private void LoadTargets(XDocument xDocument)
        {
            if (xDocument == null)
                throw new ArgumentNullException("xDocument");

            if (xDocument.Root == null)
                throw new NullReferenceException("Unable to load scenario. Root of XML file is empty.");

            foreach (XElement element in xDocument.Root.Elements("Targets").Elements())
            {
                Target target = LoadTarget();

                target.ItemType = LoadItem(element);

                target.Difficulty = LoadDifficulty(element);
                target.IsHostile = LoadHostile(element);

                List<Vector3> waypoints = LoadWaypoints(element);

                // We already spawn the waypoints during the load phase because that is easier.
                _scenario.SpawnWaypointsForTarget(target, waypoints);
                _scenario.Targets.Add(target);
            }
        }

        /// <summary>
        /// Loads item from xml
        /// </summary>
        private static ItemType LoadItem(XElement target)
        {
            ItemType item = ItemType.None;
            XElement xItem = target.Element("Item");
            if (xItem != null && Enum.IsDefined(typeof(ItemType), xItem.Value))
            {
                item = (ItemType) Enum.Parse(typeof(ItemType), xItem.Value);
            }
            return item;
        }

        /// <summary>
        /// Get difficulty selected from xml 
        /// </summary>
        private Difficulty LoadDifficulty(XElement target)
        {
            Difficulty difficulty = Difficulty.Easy;
            XElement xDifficulty = target.Element("Difficulty");
            if (xDifficulty != null && Enum.IsDefined(typeof(Difficulty), xDifficulty.Value))
            {
                difficulty = (Difficulty) Enum.Parse(typeof(Difficulty), xDifficulty.Value);
            }
            else
            {
                difficulty = this.difficulty;
            }
            return difficulty;
        }

        /// <summary>
        /// loads all waypoints from xml
        /// </summary>
        /// <remarks>
        /// The first waypoint in the list is the spawn position of a target
        /// </remarks>
        private static List<Vector3> LoadWaypoints(XElement xTarget)
        {
            List<Vector3> waypoints = new List<Vector3>();
            XElement xWaypoints = xTarget.Element("Waypoints");

            if (xWaypoints == null)
            {
                Debug.LogWarning("Unable to find Waypoints XElement in scenario.");
                return waypoints;
            }

            foreach (XElement xElement in xWaypoints.Elements())
            {
                try
                {
                    waypoints.Add(XmlToClass.CreateVector3(xElement));
                }
                catch (Exception)
                {
                    Debug.LogWarning("Unable to add Waypoint. Invalid input.");
                }
            }
            return waypoints;
        }

        /// <summary>
        /// Loads the hostile state of a target
        /// </summary>
        /// <param name="xTarget"></param>
        /// <returns></returns>
        private static bool LoadHostile(XElement xTarget)
        {
            bool isHostile = false;
            XElement xIsHostile = xTarget.Element("IsHostile");
            if (xIsHostile != null && !bool.TryParse(xIsHostile.Value, out isHostile))
            {
                Debug.LogWarning("Unable to parse IsHostile value.");
            }
            return isHostile;
        }

        /// <summary>
        /// loads the targettype of the target
        /// </summary>
        private Target LoadTarget()
        {
            Target target;
            switch (_scenario.TargetType)
            {
                case TargetType.Paper:
                    throw new NotImplementedException("I don't know how to make paper targets!");
                case TargetType.Dummy:
                    throw new NotImplementedException("I don't know how to make dummy targets!");
                case TargetType.Person:
                    target = new TargetNpc();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return target;
        }

        public void SetDifficulty(Difficulty difficulty)
        {
            this.difficulty = difficulty;
        }
    }
}