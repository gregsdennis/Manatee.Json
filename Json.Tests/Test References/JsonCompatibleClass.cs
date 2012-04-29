using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Test_References
{
	class JsonCompatibleClass : IJsonCompatible
	{
		public string StringProp { get; private set; }
		public int IntProp { get; private set; }

		public JsonCompatibleClass() {}
		public JsonCompatibleClass(string s, int i)
		{
			StringProp = s;
			IntProp = i;
		}

		#region Implementation of IJsonCompatible

		public void FromJson(JsonValue json)
		{
			StringProp = json.Object["StringProp"].String;
			IntProp = (int)json.Object["IntProp"].Number;
		}

		public JsonValue ToJson()
		{
			return new JsonObject
			       	{
			       		{"StringProp", StringProp},
			       		{"IntProp", IntProp}
			       	};
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is JsonCompatibleClass) return Equals((JsonCompatibleClass)obj);
			return base.Equals(obj);
		}

		public bool Equals(JsonCompatibleClass other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.StringProp, StringProp) && other.IntProp == IntProp;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((StringProp != null ? StringProp.GetHashCode() : 0)*397) ^ IntProp;
			}
		}
	}
}
