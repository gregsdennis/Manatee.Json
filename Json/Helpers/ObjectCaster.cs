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
 
	File Name:		ObjectCaster.cs
	Namespace:		Manatee.Json.Helpers
	Class Name:		ObjectCaster
	Purpose:		Provides type-safe generic casting with additional functionality.

***************************************************************************************/
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Manatee.Json.Helpers
{
	internal static class ObjectCaster
	{
		public static bool TryCast<T>(object obj, out T result)
		{
			try
			{
				result = (T)obj;
				return true;
			}
			catch (InvalidCastException)
			{
				result = default(T);
				var parseMethod = typeof(T).GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
													  null, new[] { typeof(string), typeof(T).MakeByRefType() }, null);
				if (parseMethod == null) return false;
				var paramsList = new object[] { obj.ToString(), result };
				if ((bool)parseMethod.Invoke(null, paramsList))
				{
					result = (T)paramsList[1];
					return true;
				}
			}
			catch
			{
				result = default(T);
			}
			return false;
		}

		public static T Cast<T>(object obj)
		{
			try
			{
				return (T) obj;
			}
			catch (InvalidCastException)
			{
				return default(T);
			}
		}
	}
}
