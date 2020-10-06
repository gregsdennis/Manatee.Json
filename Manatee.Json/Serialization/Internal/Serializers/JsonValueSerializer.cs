using System;
using System.Linq;
using JetBrains.Annotations;

namespace Manatee.Json.Serialization.Internal.Serializers
{
    [UsedImplicitly]
    internal class JsonValueSerializer : IPrioritizedSerializer
    {
        public int Priority => 2;
        public bool ShouldMaintainReferences => true;

        private Type[] HandledTypes = {
            typeof(JsonValue),
            typeof(JsonObject),
            typeof(JsonArray)
        };

        public bool Handles(SerializationContextBase context)
        {
            return HandledTypes.Contains(context.InferredType);
        }

        public JsonValue Serialize(SerializationContext context)
        {
            switch (context.Source)
            {
                case JsonValue jsonValue:
                    switch(jsonValue.Type)
                    {
                        case JsonValueType.Object:
                            return SerializeJsonObject(context, jsonValue.Object);
                        case JsonValueType.Array:
                            return SerializeJsonArray(context, jsonValue.Array);
                        case JsonValueType.Number:
                        case JsonValueType.String:
                        case JsonValueType.Boolean:
                        case JsonValueType.Null:
                        default:
                            return jsonValue;
                    }
                case JsonObject jsonObject:
                    return SerializeJsonObject(context, jsonObject);
                case JsonArray jsonArray:
                    return SerializeJsonArray(context, jsonArray);
                default:
					throw new ArgumentOutOfRangeException();
            }
        }

        private JsonValue SerializeJsonObject(SerializationContext context, JsonObject jsonObject)
        {
            var result = new JsonObject();
            foreach (var kvp in jsonObject)
            {
                var inferredValue = GetInferredValue(kvp.Value);
                context.Push(inferredValue.GetType(), typeof(JsonValue), kvp.Key, inferredValue);
                result.Add(kvp.Key, context.RootSerializer.Serialize(context));
                context.Pop();
            }
            return result;
        }

        private JsonValue SerializeJsonArray(SerializationContext context, JsonArray jsonArray)
        {
            return jsonArray.Select((value, i) =>
            {
                var inferredValue = GetInferredValue(value);
                context.Push(inferredValue.GetType(), typeof(JsonValue), i.ToString(), inferredValue);
                var json = context.RootSerializer.Serialize(context);
                context.Pop();
                return json;
            }).ToList().ToJson();
        }

        public object Deserialize(DeserializationContext context)
        {
            switch (context.LocalValue.Type)
            {
                case JsonValueType.Object:
                    return DeserializeJsonObject(context);
                case JsonValueType.Array:
                    return DeserializeJsonArray(context);
                case JsonValueType.Number:
                case JsonValueType.String:
                case JsonValueType.Boolean:
                case JsonValueType.Null:
                default:
                    return context.LocalValue;
            }
        }

        private object DeserializeJsonObject(DeserializationContext context)
        {
            var jsonObject = new JsonObject();
            foreach (var property in context.LocalValue.Object)
            {
                var inferredValue = GetInferredValue(property.Value);
                var inferredType = inferredValue.GetType();
                context.Push(inferredType, property.Key, property.Value);
                var json = context.RootSerializer.Deserialize(context);
                context.Pop();
                jsonObject[property.Key] = json != null ? ToJsonValue(json) : ToJsonValue(inferredValue);
            }

            return (context.InferredType == typeof(JsonObject))
                ? jsonObject
                : new JsonValue(jsonObject);
        }

        private object DeserializeJsonArray(DeserializationContext context)
        {
            var jsonValueEnumerable = context.LocalValue.Array.Select((value, index) =>
            {
                context.Push(typeof(JsonValue), index.ToString(), value);
                var json = context.RootSerializer.Deserialize(context);
                context.Pop();
                return json == null ? JsonValue.Null : ToJsonValue(json);
            });
            var jsonArray = new JsonArray(jsonValueEnumerable);

            return (context.InferredType == typeof(JsonArray))
                ? jsonArray
                : new JsonValue(jsonArray);
        }

        private object GetInferredValue(JsonValue value)
        {
            switch(value.Type)
            {
                case JsonValueType.Object:
                case JsonValueType.Array:
                    return value;
                default:
                    return value.GetValue();
            }
        }

        private JsonValue ToJsonValue(object value)
        {
            switch(value)
            {
                case JsonValue jsonValue:
                    return jsonValue;
                case JsonObject jsonObject:
                    return new JsonValue(jsonObject);
                case JsonArray jsonArray:
                    return new JsonValue(jsonArray);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
