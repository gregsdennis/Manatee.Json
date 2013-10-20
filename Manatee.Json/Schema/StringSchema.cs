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
 
	File Name:		StringSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		StringSchema
	Purpose:		Defines a schema which expects a string.

***************************************************************************************/
using Manatee.Json.Extensions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects a string.
	/// </summary>
	public class StringSchema : JsonSchema
	{
		/// <summary>
		/// Defines a required format for the string.
		/// </summary>
		// todo: this may need to be an enum with the valid types
		public string Format { get; set; }
		/// <summary>
		/// Defines a minimum acceptable length.
		/// </summary>
		public uint? MinLength { get; set; }
		/// <summary>
		/// Defines a maximum acceptable length.
		/// </summary>
		public uint? MaxLength { get; set; }

		/// <summary>
		/// Creates a new instance of the <see cref="StringSchema"/> class
		/// </summary>
		public StringSchema() : base(JsonSchemaTypeDefinition.String) {}

		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public override void FromJson(JsonValue json)
		{
			base.FromJson(json);
			var obj = json.Object;
			Format = obj.TryGetString("format");
			MinLength = (uint?) obj.TryGetNumber("minLength");
			MaxLength = (uint?) obj.TryGetNumber("maxLength");
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public override JsonValue ToJson()
		{
			var json = base.ToJson().Object;
			if (!string.IsNullOrWhiteSpace(Format)) json["format"] = Format;
			if (MinLength.HasValue) json["minLength"] = MinLength;
			if (MaxLength.HasValue) json["maxLength"] = MaxLength;
			return json;
		}
	}
}