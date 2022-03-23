using StateMachineMetadata.Extensions;
using StateMachineMetadata.Model;
using System;
using System.Collections.Generic;

namespace StateMachineMetadata.StateMachineCodeParts
{
    public class Header : IToCSharp
    {
        private MainModel members { get; set; }

        public IEnumerable<string> ToCSharp(MainModel model)
        {
            members = model;
            return fileHeader;
        }


        private string[] fileHeader => new string[]
            {
                @"///////////////////////////////////////////////////////////",
                @"//  Copyright © Corning Incorporated  2017",
                @"//  CStateMachineEventData.cs",
                @"//  Project CaliforniaSystem",
                @"//  Implementation of the Class CStateMachineEventData",
                @"//  Created on:      January 14, 2017 5:14:54 AM",
                @"///////////////////////////////////////////////////////////",
                @"",
                @"using System.Threading;",
                @"using Corning.GenSys.Logger;",
                @"using NorthStateSoftware.NorthStateFramework;",
                @"using System;",
                @"using System.Collections.Generic;",
                @"using California.Interfaces;",
                @"using CaliforniaLensDisplay;",
                $"namespace {members.SystemNamespace}",
                @"{",
                @"    public partial class " + members.StateMachineTypeName,
                @"    {",
            };

    }
};

