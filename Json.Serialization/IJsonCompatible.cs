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
 
	File Name:		IJsonCompatible.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		IJsonCompatible
	Purpose:		Provides implementers the option to set a preferred method
					for serialization.

***************************************************************************************/
namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides implementers the option to set a preferred method for serialization.
	/// </summary>
	public interface IJsonCompatible
	{
		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		void FromJson(JsonValue json);
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		JsonValue ToJson();
	}
}
