using System;

namespace Manatee.Json.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class SchemaAttribute : Attribute
	{
		public string Source { get; }

		public SchemaAttribute(string source)
		{
			Source = source;
		}
	}
}
