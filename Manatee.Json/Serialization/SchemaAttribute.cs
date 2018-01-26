using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Indicates that a type should be validated by a JSON Schema before deserializing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class SchemaAttribute : Attribute
	{
		/// <summary>
		/// The source of the schema.  May be an absolute URI or the name of a static property defined on the type.
		/// </summary>
		public string Source { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="SchemaAttribute"/>.
		/// </summary>
		/// <param name="source">The source of the schema.</param>
		public SchemaAttribute(string source)
		{
			Source = source;
		}
	}
}
