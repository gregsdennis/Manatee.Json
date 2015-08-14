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
 
	File Name:		MapBaseAbstractionBehavior.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		MapBaseAbstractionBehavior
	Purpose:		Describes mapping behaviors for mapping abstraction types
					in the serializer.

***************************************************************************************/

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Describes mapping behaviors for mapping abstraction types in the serializer.
	/// </summary>
	public enum MapBaseAbstractionBehavior
	{
		/// <summary>
		/// Specifies that no additional mappings will be made.
		/// </summary>
		None,
		/// <summary>
		/// Specifies that any unmapped base classes and interfaces will be mapped.
		/// </summary>
		Unmapped,
		/// <summary>
		/// Specifies that all base classes and interfaces will be mapped,
		/// overriding any existing mappings.
		/// </summary>
		Override
	}
}