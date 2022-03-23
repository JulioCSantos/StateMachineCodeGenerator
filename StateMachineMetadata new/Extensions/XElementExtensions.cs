using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StateMachineMetadata.Extensions
{
    public static class XElementExtensions
    {
        public static string GetId(this XElement elem)
        {
            return elem.Attribute("xmi.id").Value;
        }

        public static string GetName(this XElement elem)
        {
            return elem.Attribute("name")?.Value;
        }

        public static string GetOwnerId(this XElement elem)
        {
            return elem.Descendants().Where(d => d.Attribute("tag") != null && d.Attribute("tag").Value == "owner").FirstOrDefault()?.Attribute("value").Value;
        }

        public static string GetSubject(this XElement elem)
        {
            return elem.Attribute("subject")?.Value;
        }

        public static Coordinates? ToCoordinates(this XElement elem)
        {
            var geometryAttr = elem.Attribute("geometry")?.Value;
            if (geometryAttr == null) return null;
            if (!(geometryAttr.Contains("Left") && geometryAttr.Contains("Top"))) return null;
            var strCoord = geometryAttr.Split(';');
            int left, top, right, bottom = 0;
            int.TryParse(strCoord[0].Substring(5), out left);
            int.TryParse(strCoord[1].Substring(4), out top);
            int.TryParse(strCoord[2].Substring(6), out right);
            int.TryParse(strCoord[3].Substring(7), out bottom);

            return new Coordinates(left, top, right, bottom);
        }
    }
}
