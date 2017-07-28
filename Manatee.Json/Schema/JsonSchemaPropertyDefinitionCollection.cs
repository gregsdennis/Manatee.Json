using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a collection of properties within a schema.
	/// </summary>
	public class JsonSchemaPropertyDefinitionCollection : List<JsonSchemaPropertyDefinition>
	{
		/// <summary>
		/// Retrieves a schema property by name.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>The requested property or null if it does not exist.</returns>
		public JsonSchemaPropertyDefinition this[string name]
		{
			get { return this.FirstOrDefault(p => p.Name == name); }
			set
			{
				if (name == null) throw new ArgumentNullException(nameof(name));
				if (string.IsNullOrWhiteSpace(name))
					throw new ArgumentException($"Parameter '{nameof(name)}' must not be null or empty.");
				if (!string.IsNullOrWhiteSpace(value.Name))
					throw new InvalidOperationException("Cannot add a named definition using the indexer.");

				var definition = this[name];
				Remove(definition);

				value.Name = name;
				Add(value);
			}
		}
	}
}