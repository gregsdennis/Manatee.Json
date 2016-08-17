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
 
	File Name:		JsonSchemaTypeDefinitionCollection.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaTypeDefinitionCollection
	Purpose:		Defines a collection of type definitions within a schema.

***************************************************************************************/
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
		}
	}
}