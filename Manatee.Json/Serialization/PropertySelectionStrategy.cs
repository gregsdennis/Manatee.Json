/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		PropertySelectionStrategy.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		PropertySelectionStrategy
	Purpose:		Enumerates the types of properties which are automatically
					serialized.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Enumerates the types of properties which are automatically serialized.
	/// </summary>
	[Flags]
	public enum PropertySelectionStrategy
	{
		/// <summary>
		/// Indicates that read/write properties will be serialized.
		/// </summary>
		ReadWriteOnly = 1,
		/// <summary>
		/// Indicates that read-only properties will be serialized.
		/// </summary>
		ReadOnly = 2,
	}
}