using System;
using System.Collections.Generic;

namespace Manatee.Json.Patch
{
    internal static class JsonPatchExtensions
    {
        public static void Validate(this JsonPatchAction action)
        {
            var errors = new List<string>();
            switch (action.Operation)
            {
                case JsonPatchOperation.Add:
                case JsonPatchOperation.Replace:
                case JsonPatchOperation.Test:
                    action.Path._CheckProperty(action.Operation, "path", errors);
                    action.Value._CheckProperty(action.Operation, "value", errors);
                    break;
                case JsonPatchOperation.Remove:
                    action.Path._CheckProperty(action.Operation, "path", errors);
                    break;
                case JsonPatchOperation.Move:
                case JsonPatchOperation.Copy:
                    action.Path._CheckProperty(action.Operation, "path", errors);
                    action.From._CheckProperty(action.Operation, "from", errors);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void _CheckProperty(this object obj, JsonPatchOperation operation, string propertyName, List<string> errors)
        {
            if (obj != null) return;
            errors.Add($"Operation '{operation}' requires a {propertyName}.");
        }
    }
}