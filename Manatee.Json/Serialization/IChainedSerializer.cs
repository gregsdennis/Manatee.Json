using System;

namespace Manatee.Json.Serialization
{
	internal interface IChainedSerializer
	{
		JsonValue TrySerialize(ISerializer serializer, SerializationContext context);
		object? TryDeserialize(ISerializer serializer, DeserializationContext context);
	}
}