using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Test_References
{
	public class JsonSerializableClass : IJsonSerializable
	{
		public string StringProp { get; private set; }
		public int IntProp { get; private set; }

		public JsonSerializableClass() { }
		public JsonSerializableClass(string s, int i)
		{
			StringProp = s;
			IntProp = i;
		}

		#region Implementation of IJsonSerializable

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			StringProp = json.Object["StringProp"].String;
			IntProp = (int)json.Object["IntProp"].Number;
		}

		public JsonValue ToJson(JsonSerializer serializer)
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
			if (obj is JsonSerializableClass) return Equals((JsonSerializableClass)obj);
			return base.Equals(obj);
		}

		public bool Equals(JsonSerializableClass other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.StringProp, StringProp) && other.IntProp == IntProp;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((StringProp != null ? StringProp.GetHashCode() : 0) * 397) ^ IntProp;
			}
		}
	}
}