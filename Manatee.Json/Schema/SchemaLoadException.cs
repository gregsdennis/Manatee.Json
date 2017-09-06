using System;

namespace Manatee.Json.Schema
{
    public class SchemaLoadException : Exception
    {
        public SchemaLoadException(string message) : base(message) { }
    }
}