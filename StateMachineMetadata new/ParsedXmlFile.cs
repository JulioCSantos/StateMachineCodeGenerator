using StateMachineMetadata.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace StateMachineMetadata
{
    public class ParsedXmlFile
    {
        public readonly string XmlFilePath;

        public ParsedXmlFile(string xmlFilePath)
        {
            XmlFilePath = xmlFilePath;
        }

        #region Properties

        #region Root Level
        private XElement xmlRootElement;

        public XElement XmlRootElement {
            get {

                //XmlDocument xmlDocument = new XmlDocument();
                StreamReader reader = new StreamReader(XmlFilePath);
                //xmlDocument.Load(reader);
                //reader.Close();


                return xmlRootElement ?? (xmlRootElement = XElement.Load(reader));
                //return xmlRootElement ?? (xmlRootElement = XElement.Load(XmlFilePath));
            }
        }

        public XNamespace UML { get; private set; } = "omg.org/UML1.3";
        #endregion Root Level

        #region First Level
        private XElement contentElem;
        public XElement ContentElem { get { return contentElem ?? (contentElem = XmlRootElement.Element("XMI.content")); } }
        #endregion First Level

        #region Second Level
        private XElement modelElem;
        public XElement ModelElem { get { return modelElem ?? (modelElem = ContentElem.Element(UML + "Model")); } }


        private IEnumerable<XElement> diagramElems;
        public IEnumerable<XElement> DiagramNodes {
            get {
                return diagramElems ?? (diagramElems = ContentElem.Descendants(UML + "Diagram")
                    .Where(d => d.Attribute("diagramType")?.Value == "StateDiagram"));
            }
        }


        private XElement activeDiagramElem;
        public XElement ActiveDiagramElem {
            get { return activeDiagramElem; }
            set {
                //Reset dependent states
                drawnElems = null;
                activeElems = null;
                activeStatesElems = null;
                activeTransitionsElems = null;
                activeCommentsElems = null;

                activeDiagramElem = value;
            }
        } 

        //TODO: EANoteLink

        #endregion Second Level

        #region Third Level
        //TODO: documentation

        //TODO: Namespace.ownedElement

        //TODO: Diagram.element
        #endregion Third Level


        #region Of Interest elements

        private IEnumerable<XElement> drawnElems;
        public IEnumerable<XElement> DrawnElems { get { return drawnElems ?? (drawnElems = ActiveDiagramElem.Descendants(UML + "DiagramElement")); } }


        private IEnumerable<XElement> primaryElems;
        public IEnumerable<XElement> PrimaryElems {
            get
            {
                if (primaryElems == null)
                {
                    primaryElems = ContentElem.Descendants()
                    .Where(d => d.Attribute("xmi.id") != null &&
                        (d.Name.LocalName == "SimpleState" || d.Name.LocalName == "PseudoState" 
                        || d.Name.LocalName == "CompositeState" || d.Name.LocalName == "Transition" || d.Name.LocalName == "Comment")
                    );
                }
                return primaryElems;
            }
        }


        private IEnumerable<XElement> activeElems;
        public IEnumerable<XElement> ActiveElems
        {
            get
            {
                if (activeElems == null)
                {
                    activeElems = DrawnElems
                        .Join(PrimaryElems, de => de.Attribute("subject")?.Value, ce => ce.Attribute("xmi.id").Value, (de, ce) => ce);
                }
                return activeElems;
            }
        }

        private IEnumerable<XElement> activeStatesElems;
        public IEnumerable<XElement> ActiveStatesElems
        {
            get { return activeStatesElems ?? (activeStatesElems = ActiveElems.Where(d => d.Name.LocalName == "SimpleState" || d.Name.LocalName == "PseudoState" || d.Name.LocalName == "CompositeState")); }
        }

        private IEnumerable<XElement> activeTransitionsElems;
        public IEnumerable<XElement> ActiveTransitionsElems
        {
            get { return activeTransitionsElems ?? (activeTransitionsElems = ActiveElems.Where(d => d.Name.LocalName == "Transition")); }
        }


        private IEnumerable<XElement> activeCommentsElems;
        public IEnumerable<XElement> ActiveCommentsElems
        {
            get { return activeCommentsElems ?? (activeCommentsElems = ActiveElems.Where(d => d.Name.LocalName == "Comment")); }
        }


        #endregion Of Interest elements


        #endregion Properties
    }
}
