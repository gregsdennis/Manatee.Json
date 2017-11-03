using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class ExpandoObjectSerializationDeleteProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type == typeof(ExpandoObject);
		}

		private static JsonValue _Encode(ExpandoObject input, JsonSerializer serializer)
		{
			var dict = (IDictionary<string, object>) input;
			return dict.ToDictionary(kvp => kvp.Key, kvp => serializer.Serialize<dynamic>(kvp.Value))
			           .ToJson();
		}
		private static ExpandoObject _Decode(JsonValue json, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}