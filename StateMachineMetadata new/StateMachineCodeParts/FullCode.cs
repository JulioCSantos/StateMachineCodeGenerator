using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.StateMachineCodeParts
{
    public class FullCode : IToCSharp
    {
        //public MainModel Model { get; private set; }
        //public FullCode(MainModel model)
        //{
        //    Model = model;
        //}

        //public Header Header { get { return new Header(); } }
        //public Body Body { get { return new Body(); } }
        //public Trailer Trailer { get { return new Trailer(); } }

        public IEnumerable<string> ToCSharp(MainModel model)
        {
            var fullCode = (new Header()).ToCSharp(model).ToList();
            fullCode.AddRange((new Body()).ToCSharp(model));
            fullCode.AddRange((new Trailer()).ToCSharp(model));
            return fullCode;
        }
    }
}
