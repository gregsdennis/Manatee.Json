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
 
	File Name:		MultiSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		MultiSchema
	Purpose:		Provides a mechanism to define a mutli-type schema.

***************************************************************************************/
namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides a mechanism to define a mutli-type schema.
	/// </summary>
	public class MultiSchema : JsonSchema
	{
		/// <summary>
		/// Creates an instance of the <see cref="MultiSchema"/> class.
		/// </summary>
		/// <param name="schemata">The collection of <see cref="IJsonSchema"/> objects.</param>
		public MultiSchema(params IJsonSchema[] schemata)
		{
			Type = new JsonSchemaMultiTypeDefinition(schemata);
		}
	}
}