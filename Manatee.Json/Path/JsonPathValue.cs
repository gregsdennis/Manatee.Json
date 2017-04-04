using System;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Serves as a stand-in for JsonValue in Path Expressions.
	/// </summary>
#pragma warning disable 660,661
	public sealed class JsonPathValue
#pragma warning restore 660,661
	{
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