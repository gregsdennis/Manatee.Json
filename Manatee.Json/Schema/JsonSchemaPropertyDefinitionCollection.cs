/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSchemaPropertyDefinitionCollection.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaPropertyDefinitionCollection
	Purpose:		Defines a collection of properties within a schema.

***************************************************************************************/
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
		}
	}
}