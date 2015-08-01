using Manatee.Json.Serialization;

namespace Manatee.Json.Performance
{
	public class Associate
	{
		[JsonMapTo("id")]
		public string Id { get; set; }

		public string FamilyName { get; set; }

		public string GivenName { get; set; }
	}

	public class SerializableAssociate : IJsonSerializable
	{
		public string Id { get; set; }

		public SerializableName Name { get; set; }
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Id = json.Object["id"].String;
			Name = serializer.Deserialize<SerializableName>(json);
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var name = serializer.Serialize(Name);
			var obj = new JsonObject
				{
					{"id", Id},
				};
			foreach (var kvp in name.Object)
			{
				obj[kvp.Key] = kvp.Value;
			}
			return obj;
		}
	}

	public class SerializableName : IJsonSerializable
	{
		public string Family { get; set; }

		public string Given { get; set; }
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Family = json.Object["familyName"].String;
			Given = json.Object["givenName"].String;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject
				{
					{"familyName", Family},
					{"givenName", Given},
				};
		}
	}
}
