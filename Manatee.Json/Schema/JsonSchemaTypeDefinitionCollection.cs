﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a collection of type definitions within a schema.
	/// </summary>
	public class JsonSchemaTypeDefinitionCollection : List<JsonSchemaTypeDefinition>
	{
		/// <summary>
		/// Retrieves a schema type definition by name.
		/// </summary>
		/// <param name="name">The name of the type definition.</param>
		/// <returns>The requested type definition or null if it does not exist.</returns>
		public JsonSchemaTypeDefinition this[string name]
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