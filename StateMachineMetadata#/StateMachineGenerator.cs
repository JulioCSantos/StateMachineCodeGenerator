using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StateMachineMetadata.StateMachineCodeParts;
using StateMachineMetadata.Model;

namespace StateMachineMetadata
{
    public static class StateMachineGenerator
    {
        public static MainModel Model { get; private set; }

        public static void Generate(MainModel model, string toPath)
        {
            Model = model;
            var stateMachineCode = new FullCode();
            var toStateMachine = $"{toPath}\\{model.StateMachineTypeName}.cs";
            System.IO.File.WriteAllLines(toStateMachine, stateMachineCode.ToCSharp(Model));
            //var toZacksFeed = $"{toPath}\\{model.ZacksFeed}.cs";
            //System.IO.File.WriteAllLines(toZacksFeed, (new ZacksGeneratorFeed()).ToCSharp(Model));
        }
    }
}
