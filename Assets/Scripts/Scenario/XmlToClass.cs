using System;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// convert XML elements to correct type
    /// </summary>
    public class XmlToClass
    {
        /// <summary>
        /// create vector3 from element
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static Vector3 CreateVector3(XElement xElement)
        {
            if (xElement == null)
                throw new ArgumentNullException("xElement");

            var x = CreateFloat(xElement.Attribute("x"));
            var y = CreateFloat(xElement.Attribute("y"));
            var z = CreateFloat(xElement.Attribute("z"));

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// create float from attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static float CreateFloat(XAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            float f;
            if (!float.TryParse(attribute.Value, out f))
                throw new Exception("Unable to convert attribute value to float.");

            return f;
        }
    }
}