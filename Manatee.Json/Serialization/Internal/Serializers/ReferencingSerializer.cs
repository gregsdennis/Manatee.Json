using System;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class ReferencingSerializer : IChainedSerializer
	{
		public static ReferencingSerializer Instance { get; } = new ReferencingSerializer();

		private ReferencingSerializer() { }

		public bool Handles(ISerializer serializer, Type type)
		{
			return !type.IsValueType && serializer.ShouldMaintainReferences;
		}
		public JsonValue TrySerialize(ISerializer serializer, SerializationContext context)
		{
			if (context.SerializationMap.TryGetPair(context.Source!, out var pair))
				return new JsonObject {[Constants.RefKey] = pair.Source.ToString()};

			context.SerializationMap.Add(new SerializationReference
				{
					Object = context.Source,
					Source = context.CurrentLocation.CleanAndClone()
				});

			return serializer.Serialize(context);
		}
		public object? TryDeserialize(ISerializer serializer, DeserializationContext context)
		{
			if (context.LocalValue.Type == JsonValueType.Object)
			{
				var jsonObj = context.LocalValue.Object;
				if (jsonObj.TryGetValue(Constants.RefKey, out var reference))
				{
					var location = JsonPointer.Parse(reference.String);
					context.SerializationMap.AddReference(location, context.CurrentLocation.CleanAndClone());
					return context.InferredType.Default();
				}
			}

			var pair = new SerializationReference
				{
					Source = context.CurrentLocation.CleanAndClone()
				};
			context.SerializationMap.Add(pair);

			var obj = serializer.Deserialize(context);

			pair.Object = obj;
			pair.DeserializationIsComplete = true;

			return obj;
		}
	}
}