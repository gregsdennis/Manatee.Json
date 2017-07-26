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
				var definition = this.FirstOrDefault(p => p.Name == name);
				if (definition != null)
					Remove(definition);
				value.Name = name;
				Add(value);
			}
		}
	}
}