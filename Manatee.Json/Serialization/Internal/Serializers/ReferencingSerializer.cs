using Manatee.Json.Internal;
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
		public JsonValue Serialize(SerializationContext context)
		{
			if (context.RootSerializer.SerializationMap.TryGetPair(context.Source, out var pair))
				return new JsonObject {{Constants.RefKey, pair.Source.ToString()}};

			context.RootSerializer.SerializationMap.Add(new SerializationReference
				{
					Object = context.Source,
					Source = context.CurrentLocation
				});

			return _innerSerializer.Serialize(context);
		}
		public object Deserialize(SerializationContext context)
		{
			if (context.LocalValue.Type == JsonValueType.Object)
			{
				var jsonObj = context.LocalValue.Object;
				if (jsonObj.TryGetValue(Constants.RefKey, out var reference))
				{
					var location = JsonPointer.Parse(reference.String);
					context.RootSerializer.SerializationMap.AddReference(location, context.CurrentLocation);
					return context.InferredType.Default();
				}
			}

			var pair = new SerializationReference
				{
					Source = context.CurrentLocation
				};
			context.RootSerializer.SerializationMap.Add(pair);

			var obj = _innerSerializer.Deserialize(context);

			pair.Object = obj;
			pair.DeserializationIsComplete = true;

			return obj;
		}
	}
}