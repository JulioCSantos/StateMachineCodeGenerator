using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;


namespace StateMachineMetadata.StateMachineCodeParts
{
    public class Trailer : IToCSharp
    {
        private MainModel members { get; set; }

        public IEnumerable<string> ToCSharp(MainModel model)
        {
            members = model;
            return filetrailer;
        }

        private string[] filetrailer => new string[]
        {
            "    } //end " + members.StateMachineTypeName,
            "} //end " + members.SystemNamespace 
        };
}
}