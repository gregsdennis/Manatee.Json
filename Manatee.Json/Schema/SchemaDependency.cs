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
 
	File Name:		SchemaDependency.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		SchemaDependency
	Purpose:		Creates a dependency that is based on a secondary schema.

***************************************************************************************/
using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Creates a dependency that is based on a secondary schema.
	/// </summary>
	public class SchemaDependency : IJsonSchemaDependency
	{
		private readonly IJsonSchema _schema;

		/// <summary>
		/// Gets or sets the property with the dependency.
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="SchemaDependency"/> class.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <param name="schema">The schema which must be validated.</param>
		public SchemaDependency(string propertyName, IJsonSchema schema)
		{
			_schema = schema;
			if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
			if (propertyName.IsNullOrWhiteSpace()) throw new ArgumentException("Must provide a property name.");

			PropertyName = propertyName;
		}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>The results of the validation.</returns>
		public SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			return _schema.Validate(json, root);
		}
		/// <summary>
		/// Gets the JSON data to be used as the value portion in the dependency list of the schema.
		/// </summary>
		public JsonValue GetJsonData()
		{
			return _schema.ToJson(null);
		}
	}
}