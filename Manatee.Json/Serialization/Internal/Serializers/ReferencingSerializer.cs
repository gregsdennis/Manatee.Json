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

		public bool Handles(SerializationContext context)
		{
			return true;
		}
		public JsonValue Serialize<T>(SerializationContext<T> context)
		{
			if (context.RootSerializer.SerializationMap.TryGetPair(context.Source, out var pair))
				return new JsonObject {{Constants.RefKey, pair.Reference.ToString()}};

			context.RootSerializer.SerializationMap.Add(new SerializationReference
				{
					Object = context.Source,
					Reference = context.CurrentLocation
				});

			return _innerSerializer.Serialize(context);
		}
		public T Deserialize<T>(SerializationContext<JsonValue> context)
		{
			if (context.Source.Type == JsonValueType.Object)
			{
				var jsonObj = context.Source.Object;
				if (jsonObj.TryGetValue(Constants.RefKey, out var reference))
				{
					var location = JsonPointer.Parse(reference.String);
					if (context.RootSerializer.SerializationMap.TryGetPair(location, out var pair))
						return (T) pair.Object;
				}
			}

			context.RootSerializer.SerializationMap.Add(new SerializationReference
				{
					Json = context.Source,
					Reference = context.CurrentLocation
				});

			return _innerSerializer.Deserialize<T>(context);
		}
	}
}