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
		private class OneOfSchema : IJsonSchema
		{
			private readonly IEnumerable<IJsonSchema> _definitions;

			public Uri DocumentPath { get; set; }
			public string Id { get; set; }
			public string Schema { get; set; }
		
			public OneOfSchema(IEnumerable<IJsonSchema> definitions)
			{
				_definitions = definitions.ToList();
			}

			public void FromJson(JsonValue json, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
			public JsonValue ToJson(JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
			public bool Equals(IJsonSchema other)
			{
				throw new NotImplementedException();
			}
			public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
			{
				var errors = _definitions.Select(s => s.Validate(json, root)).ToList();
				var validCount = errors.Count(r => r.Valid);
				switch (validCount)
				{
					case 0:
						return new SchemaValidationResults(errors);
					case 1:
						return new SchemaValidationResults();
					default:
						return new SchemaValidationResults(string.Empty, "More than one option was valid.");
				}
			}
		}
		
		private readonly bool _nonPrimitiveAllowed;

		internal IEnumerable<JsonSchemaTypeDefinition> Definitions { get; private set; }
		internal bool IsPrimitive => !_nonPrimitiveAllowed; 

		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaMultiTypeDefinition"/> class.
		/// </summary>
		public JsonSchemaMultiTypeDefinition(params JsonSchemaTypeDefinition[] definitions)
			: this(false)
		{
			Definitions = definitions.ToList();

			if (Definitions.Except(PrimitiveDefinitions).Any())
				throw new InvalidOperationException("Only primitive types are allowed in type collections.");

			Definition = new OneOfSchema(Definitions.Select(d => d.Definition));
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
			Definitions = typeEntry.Select(jv =>
				{
					if (_nonPrimitiveAllowed) return new JsonSchemaTypeDefinition(JsonSchemaFactory.FromJson(jv));
					var definition = PrimitiveDefinitions.FirstOrDefault(p => p.Name == jv.String);
					if (definition == null)
						throw new InvalidOperationException("Only primitive types are allowed in type collections.");
					return definition;
				}).ToList();

			Definition = new OneOfSchema(Definitions.Select(d => d.Definition));
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			return Definitions.ToJson(serializer);
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
			return _Equals(obj as JsonSchemaMultiTypeDefinition);
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
				return (base.GetHashCode()*397) ^ (Definitions?.GetCollectionHashCode() ?? 0);
			}
		}

		private bool _Equals(JsonSchemaMultiTypeDefinition other)
		{
		    if (ReferenceEquals(null, other)) return false;
		    if (ReferenceEquals(this, other)) return true;
		    if (other.GetType() != GetType()) return false;
			return Definitions.ContentsEqual(other.Definitions);
		}
	}
}