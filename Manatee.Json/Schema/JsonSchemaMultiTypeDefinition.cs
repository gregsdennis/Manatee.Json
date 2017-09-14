using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Supports a collection of schema types.
	/// </summary>
	public class JsonSchemaMultiTypeDefinition : JsonSchemaTypeDefinition
	{
		private readonly bool _nonPrimitiveAllowed;

		internal IEnumerable<JsonSchemaTypeDefinition> Defintions { get; private set; }
		internal bool IsPrimitive => !_nonPrimitiveAllowed; 

		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaMultiTypeDefinition"/> class.
		/// </summary>
		public JsonSchemaMultiTypeDefinition(params JsonSchemaTypeDefinition[] definitions)
			: this(false)
		{
			Defintions = definitions.ToList();

			if (Defintions.Except(PrimitiveDefinitions).Any())
				throw new InvalidOperationException("Only primitive types are allowed in type collections.");

			Definition = new JsonSchema04 { OneOf = Defintions.Select(d => d.Definition) };
		}
		internal JsonSchemaMultiTypeDefinition(bool nonPrimitiveAllowed)
		{
			_nonPrimitiveAllowed = nonPrimitiveAllowed;
		}

		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public override void FromJson(JsonValue json, JsonSerializer serializer)
		{
			var typeEntry = json.Array;
			Defintions = typeEntry.Select(jv =>
				{
					if (_nonPrimitiveAllowed) return new JsonSchemaTypeDefinition(JsonSchemaFactory.FromJson(jv));
					var definition = PrimitiveDefinitions.FirstOrDefault(p => p.Name == jv.String);
					if (definition == null)
						throw new InvalidOperationException("Only primitive types are allowed in type collections.");
					return definition;
				}).ToList();

			Definition = new JsonSchema04 {OneOf = Defintions.Select(d => d.Definition)};		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			return Defintions.ToJson(serializer);
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((JsonSchemaMultiTypeDefinition) obj);
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode()*397) ^ (Defintions?.GetCollectionHashCode() ?? 0);
			}
		}

		private bool Equals(JsonSchemaMultiTypeDefinition other)
		{
			return base.Equals(other) && Defintions.ContentsEqual(other.Defintions);
		}
	}
}