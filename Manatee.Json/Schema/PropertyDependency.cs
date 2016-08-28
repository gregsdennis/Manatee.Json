/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		PropertyDependency.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		PropertyDependency
	Purpose:		Declares a dependency that is based on the presence of other
					properties in the JSON.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Declares a dependency that is based on the presence of other properties in the JSON.
	/// </summary>
	public class PropertyDependency : IJsonSchemaDependency
	{
		private readonly IEnumerable<string> _dependencies;

		/// <summary>
		/// Gets or sets the property with the dependency.
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="PropertyDependency"/> class.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="dependencies"></param>
		public PropertyDependency(string propertyName, IEnumerable<string> dependencies)
		{
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (propertyName.IsNullOrWhiteSpace()) throw new ArgumentException("Must provide a property name.");
			if (dependencies == null) throw new ArgumentNullException(nameof(dependencies));
			if (!dependencies.Any()) throw new ArgumentException("Cannot create property dependency on no properties.");

			PropertyName = propertyName;
			_dependencies = dependencies.ToList();
		}
		/// <summary>
		/// Creates a new instance of the <see cref="PropertyDependency"/> class.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="firstDependency">A minimal required property dependency.</param>
		/// <param name="otherDependencies">Additional property dependencies.</param>
		public PropertyDependency(string propertyName, string firstDependency, params string[] otherDependencies)
			: this(propertyName, new[] {firstDependency}.Concat(otherDependencies)) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var errors = new List<SchemaValidationError>();
			if (json.Type == JsonValueType.Object && json.Object.ContainsKey(PropertyName))
			{
				errors.AddRange(_dependencies.Except(json.Object.Keys)
				                             .Select(d =>new SchemaValidationError(string.Empty, $"When property '{PropertyName}' exists, property '{d}' should exist as well.")));
			}
			return new SchemaValidationResults(errors);
		}
		/// <summary>
		/// Gets the JSON data to be used as the value portion in the dependency list of the schema.
		/// </summary>
		public JsonValue GetJsonData()
		{
			return _dependencies.ToJson();
		}
	}
}