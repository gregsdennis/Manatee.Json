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
 
	File Name:		JsonPathValue.cs
	Namespace:		Manatee.Json.Path
	Class Name:		JsonPathValue
	Purpose:		Serves as a stand-in for JsonValue in Path Expressions.

***************************************************************************************/

using System;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Serves as a stand-in for JsonValue in Path Expressions.
	/// </summary>
	public class JsonPathValue
	{
		/// <summary>
		/// Determines whether the specified <see cref="JsonPathValue"/> is equal to the current <see cref="JsonPathValue"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="JsonPathValue"/> is equal to the current <see cref="JsonPathValue"/>; otherwise, false.
		/// </returns>
		/// <param name="other">The <see cref="JsonPathValue"/> to compare with the current <see cref="JsonPathValue"/>. </param><filterpriority>2</filterpriority>
		protected bool Equals(JsonPathValue other)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(JsonPathValue a, string b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(string a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(JsonPathValue a, bool b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator ==(bool a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(JsonPathValue a, string b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(string a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(JsonPathValue a, bool b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator !=(bool a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <(JsonPathValue a, string b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <(string a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <=(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <=(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <=(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <=(JsonPathValue a, string b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator <=(string a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >(JsonPathValue a, string b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >(string a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >=(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >=(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >=(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >=(JsonPathValue a, string b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		///<summary>
		///</summary>
		///<param name="a"></param>
		///<param name="b"></param>
		///<returns></returns>
		public static bool operator >=(string a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static bool operator !(JsonPathValue v)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator +(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator +(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator +(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator +(JsonPathValue a, string b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator +(string a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator -(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator -(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator -(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator *(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator *(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator *(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator /(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator /(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator /(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator %(JsonPathValue a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator %(JsonPathValue a, double b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
		/// <summary>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double operator %(double a, JsonPathValue b)
		{
			throw new InvalidOperationException("This operation is reserved for JsonPath.");
		}
	}
}