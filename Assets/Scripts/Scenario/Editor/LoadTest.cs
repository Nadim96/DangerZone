using System.Xml.Linq;
using Assets.Scripts.Items;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    public class LoadTest
    {
        private const string XML_XML_PERSON_TEST = "<Target>\r\n  <Position x=\"5\" y=\"1\" z=\"5\" />\r\n  <Item>P99</Item>\r\n  <IsHostile>True</IsHostile>\r\n  <Waypoints>\r\n    <Waypoint x=\"10\" y=\"1\" z=\"10\" />\r\n  </Waypoints>\r\n</Target>";

        private Target NewTarget()
        {
            Target t = new TargetNpc();
            t.IsHostile = true;
            t.ItemType = ItemType.P99;
            t.Position = new Vector3(5, 1, 5);

            Waypoint waypoint = new GameObject().AddComponent<Waypoint>();
            waypoint.Owner = t;
            waypoint.Position = new Vector3(10, 1, 10);
            t.Waypoints.Add(waypoint);

            return t;
        }

        [Test] // only xml person is tested because Save() doesnt return xmldocument
        public void XmlPerson()
        {
            Target target = NewTarget();
            XElement test = Save.ToXElement(target);
            Assert.AreEqual(test.ToString(), XML_XML_PERSON_TEST);
        }
    }
}
