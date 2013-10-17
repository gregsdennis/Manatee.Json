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
 
	File Name:		InvalidPropertyKeyBehavior.cs
	Namespace:		Manatee.Json.Serialization.Enumerations
	Class Name:		InvalidPropertyKeyBehavior
	Purpose:		Enumeration of behavior options for the deserializer when a
					JSON structure is passed which contains invalid property keys.

***************************************************************************************/
namespace Manatee.Json.Enumerations
{
	/// <summary>
	/// Enumeration of behavior options for the deserializer when a JSON structure is passed which
	/// contains invalid property keys.
	/// </summary>
	public enum InvalidPropertyKeyBehavior
	{
		/// <summary>
		/// Deserializer ignores the invalid property keys.
		/// </summary>
		DoNothing,
		/// <summary>
		/// Deserializer will throw an exception when an invalid property key is found.
		/// </summary>
		ThrowException
	}
}