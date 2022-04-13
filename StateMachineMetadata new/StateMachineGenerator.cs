using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StateMachineMetadata.StateMachineCodeParts;
using StateMachineMetadata.Model;
using System.IO;

namespace StateMachineMetadata
{
    public class StateMachineGenerator
    {
        public MainModel Model { get; private set; }

        public void Generate(MainModel model, string toPath)
        {
            Model = model;
            var stateMachineCode = new FullCode();
            Directory.CreateDirectory(toPath);
            var toStateMachine = $"{toPath}\\{model.StateMachineTypeName}.cs";
            System.IO.File.WriteAllLines(toStateMachine, stateMachineCode.ToCSharp(Model));
        }
    }
}
