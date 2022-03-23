using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachineMetadata.Model
{
    public enum ErrorSeverity
    {
        None,
        Informational,
        Warning,
        Error
    }
    public class ErrorMessage
    {
        public List<EntityBase> InvalidEntities { get; } = new List<EntityBase>();
        public string Message { get; set; }

        public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;
        public ErrorMessage() { }
        public ErrorMessage(string message) { Message = message; }

        public ErrorMessage(EntityBase invalidEntity, string message)
        {
            InvalidEntities.Add(invalidEntity);
            Message = message;
        }

        public ErrorMessage(List<EntityBase> invalidEntities, string errorMessage)
        {
            InvalidEntities.AddRange(invalidEntities);
            Message = errorMessage;
        }

        public string ToEntitiesList()
        {
            var names = string.Join(", ", InvalidEntities.Select(ie => ie.Name));
            return names;
        }

    }
}
