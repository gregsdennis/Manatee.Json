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
 
	File Name:		GeneralExtensions.cs
	Namespace:		Manatee.Json.Internal
	Class Name:		GeneralExtensions
	Purpose:		General-use extension methods for the library.

***************************************************************************************/

using System;

namespace Manatee.Json.Internal
{
	internal static class GeneralExtensions
	{
		public static bool IsInt(this double value)
		{
			return Math.Ceiling(value) == Math.Floor(value);
		}
#if NET35 || NET35C
		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrEmpty(value.Trim());
		}
#endif
	}
}