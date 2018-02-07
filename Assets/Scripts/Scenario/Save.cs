using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// Converts the current scenario into an XML file and stores it on persistent storage.
    /// </summary>
    /// <remarks>Everything not saved will be lost - Nintendo "Quit screen" message </remarks>
    public static class Save
    {
        public static void SaveScenario(BunkerScenario scenario)
        {
            XElement xTargets = new XElement("Targets");
            foreach (Target target in scenario.Targets)
            {
                xTargets.Add(ToXElement(target));
            }

            XDocument xDocument = new XDocument
            (
                new XElement("Scenario",
                    new XElement("Name", scenario.Name),
                    new XElement("Settings",
                        new XElement("TargetType", scenario.TargetType.ToString()),
                        new XElement("GoalType", scenario.GoalType.ToString())
                    ),
                    xTargets
                )
            );

            // Store the scenario with a date-time as filename to reduce chance of duplicate filenames
            // Also chose to omit scenario name to avoid illegal characters in file path
            xDocument.Save(string.Format("{0}/Scenarios/{1:d_M_yyyy HH_mm_ss}.xml",
                Application.streamingAssetsPath, DateTime.UtcNow));
        }

        public static XElement ToXElement(Target target)
        {
            return new XElement("Target",
                new XElement("Item", target.ItemType.ToString()),
                new XElement("Difficulty", target.Difficulty.ToString()),
                new XElement("IsHostile", target.IsHostile.ToString()),
                WaypointsToXElement(target.Waypoints)
            );
        }

        /// <summary>
        /// Convert Vector3 to XElement
        /// </summary>
        /// <param name="name">name of XElement</param>
        /// <param name="vector3"></param>
        /// <returns></returns>
        private static XElement Vector3ToXElement(string name, Vector3 vector3)
        {
            return new XElement(name,
                new XAttribute("x", vector3.x),
                new XAttribute("y", vector3.y),
                new XAttribute("z", vector3.z)
            );
        }

        /// <summary>
        /// Convert all Waypoints to XElement. 
        /// </summary>
        private static XElement WaypointsToXElement(List<Waypoint> waypoints)
        {
            XElement waypointXElement = new XElement("Waypoints");

            foreach (Waypoint waypoint in waypoints)
            {
                waypointXElement.Add(Vector3ToXElement("Waypoint", waypoint.Position));
            }
            return waypointXElement;
        }
    }
}