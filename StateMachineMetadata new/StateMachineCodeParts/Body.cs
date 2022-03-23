using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachineMetadata.StateMachineCodeParts
{

    public class Body : IToCSharp
    {
        public IEnumerable<string> ToCSharp(MainModel model)
        {
            var bodyCode = (new Fields()).ToCSharp(model);
            return bodyCode;

            //var classes = StateMachineGenerator.ParsedXmlFile.ClassesElems.Select(c => new XmlClass() { Name = c.Attribute("name").Value, Id = c.Attribute("xmi.id").Value });
            //var CodeLines = classes.Select(c => @"//" + c.ToString());
            //return CodeLines;

        }
    }
}