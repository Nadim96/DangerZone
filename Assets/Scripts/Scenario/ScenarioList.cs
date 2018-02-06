using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// handles load screen of a scenario
    /// </summary>
    public static class ScenarioList
    {
        /// <summary>
        /// Returns the names and filepaths of all scenario's.
        /// </summary>
        /// <param name="names"></param>
        /// <param name="paths"></param>
        public static void GetScenarioNames(out List<string> names, out List<string> paths)
        {
            names = new List<string>();
            paths = new List<string>();

            DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Scenarios");
            FileInfo[] info = dir.GetFiles("*.xml");
            foreach (FileInfo f in info)
            {
                XDocument xDocument = XDocument.Load(f.FullName);

                if (xDocument.Root == null)
                    throw new NullReferenceException("Unable to load scenario. Root of XML file is empty.");

                XElement xName = xDocument.Root.Element("Name");
                if (xName != null && !xName.IsEmpty)
                {
                    names.Add(xName.Value);
                    paths.Add(f.FullName);
                }
            }
        }
    }
}
