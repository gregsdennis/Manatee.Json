﻿using System;

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

		public JsonValue Serialize<T>(T obj, JsonSerializer serializer)
		{
			SerializationPair pair;
			if (serializer.SerializationMap.Contains(obj))
			{
				var guid = serializer.SerializationMap.GetKey(obj);
				pair = serializer.SerializationMap[guid];
				pair.UsageCount++;
				return new JsonObject {{Constants.RefKey, guid.ToString()}};
			}
			if (_innerSerializer.ShouldMaintainReferences)
			{
				var guid = Guid.NewGuid();
				pair = new SerializationPair {Object = obj};
				serializer.SerializationMap.Add(guid, pair);
				pair.Json = _innerSerializer.Serialize(obj, serializer);
				if (pair.UsageCount != 0)
					pair.Json.Object.Add(Constants.DefKey, guid.ToString());
				return pair.Json;
			}
			return _innerSerializer.Serialize(obj, serializer);
		}
		public T Deserialize<T>(JsonValue json, JsonSerializer serializer)
		{
			if (json.Type == JsonValueType.Object)
			{
				var jsonObj = json.Object;
				SerializationPair pair;
				if (jsonObj.ContainsKey(Constants.DefKey))
				{
					var guid = new Guid(jsonObj[Constants.DefKey].String);
					jsonObj.Remove(Constants.DefKey);
					pair = new SerializationPair {Json = json};
					serializer.SerializationMap.Add(guid, pair);
					pair.Object = _innerSerializer.Deserialize<T>(json, serializer);
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
			return _innerSerializer.Deserialize<T>(json, serializer);
		}
	}
}