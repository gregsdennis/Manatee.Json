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
 
	File Name:		JsonSerializerOptions.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonSerializerOptions
	Purpose:		Default options used by the serializer.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Serialization.Enumerations;

namespace Manatee.Json.Serialization
{
	class JsonSerializerOptions
	{
		/// <summary>
		/// Default options used by the serializer.
		/// </summary>
		public JsonSerializerOptions Default = new JsonSerializerOptions
		                                       	{
		                                       		EncodeDefaultValues = false,
													InvalidPropertyKeyBehavior = InvalidPropertyKeyBehavior.DoNothing
		                                       	};
		/// <summary>
		/// Gets and sets whether the serializer encodes default values for properties.
		/// </summary>
		/// <remarks>
		/// Setting to 'true' may significantly increase the size of the JSON structure.
		/// </remarks>
		public bool EncodeDefaultValues { get; set; }
		/// <summary>
		/// Gets and sets the behavior of the deserializer when a JSON structure is passed which
		/// contains invalid property keys.
		/// </summary>
		public InvalidPropertyKeyBehavior InvalidPropertyKeyBehavior { get; set; }
	}
}
