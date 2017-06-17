using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Performance
{
	public class Associate
	{
		public string Id { get; set; }

		public string FamilyName { get; set; }

		public string GivenName { get; set; }

		public override string ToString()
		{
			return string.Format("{0} - {1} {2}", Id, GivenName, FamilyName);
		}
	}

	public class SerializableAssociate : IJsonSerializable
	{
		public string Id { get; set; }

		public string FamilyName { get; set; }

		public string GivenName { get; set; }

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Id = json.Object["id"].String;
			FamilyName = json.Object["familyName"].String;
			GivenName = json.Object["givenName"].String;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			var obj = new JsonObject
				{
					{"id", Id},
					{"familyName", FamilyName},
					{"givenName", GivenName},
				};
			return obj;
		}
		public override string ToString()
		{
			return string.Format("{0} - {1} {2}", Id, GivenName, FamilyName);
		}
	}
}
