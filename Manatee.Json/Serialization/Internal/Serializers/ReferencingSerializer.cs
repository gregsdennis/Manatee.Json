using System;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ReferencingSerializer : ISerializer
	{
		private readonly ISerializer _innerSerializer;

		public bool ShouldMaintainReferences => true;

		public ReferencingSerializer(ISerializer innerSerializer)
		{
			_innerSerializer = innerSerializer;
		}

		public bool Handles(SerializationContext context, JsonSerializerOptions options)
		{
			return true;
		}
		public JsonValue Serialize<T>(SerializationContext<T> context, JsonPointer location)
		{
			if (serializer.SerializationMap.TryGetPair(obj, out var pair))
			{
				pair.UsageCount++;
				return new JsonObject {{Constants.RefKey, guid.ToString()}};
			}

			guid = Guid.NewGuid();
			pair = new SerializationReference {Object = obj};
			serializer.SerializationMap.Add(guid, pair);
			pair.Json = _innerSerializer.Serialize(context, location);
			if (pair.UsageCount != 0)
				pair.Json.Object.Add(Constants.DefKey, guid.ToString());
			return pair.Json;
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context, JsonValue root)
		{
			if (json.Type == JsonValueType.Object)
			{
				var jsonObj = json.Object;
				SerializationReference pair;
				if (jsonObj.ContainsKey(Constants.DefKey))
				{
					var guid = new Guid(jsonObj[Constants.DefKey].String);
					jsonObj.Remove(Constants.DefKey);
					pair = new SerializationReference {Json = json};
					serializer.SerializationMap.Add(guid, pair);
					serializer.SerializationMap.Update(pair, _innerSerializer.Deserialize<T>(context, root));
					pair.DeserializationIsComplete = true;
					pair.Reconcile();
					return (T) pair.Object;
				}
				if (jsonObj.ContainsKey(Constants.RefKey))
				{
					var guid = new Guid(jsonObj[Constants.RefKey].String);
					pair = serializer.SerializationMap[guid];
					return (T) pair.Object;
				}
			}
			return _innerSerializer.Deserialize<T>(context, root);
		}
	}
}