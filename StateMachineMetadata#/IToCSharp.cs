using StateMachineMetadata.Model;
using System.Collections;
using System.Collections.Generic;

namespace StateMachineMetadata
{
    public interface IToCSharp
    {
        IEnumerable<string> ToCSharp(MainModel model);
    }
}