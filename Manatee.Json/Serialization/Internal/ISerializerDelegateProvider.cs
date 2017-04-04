using System;

namespace Manatee.Json.Serialization.Internal
{
	internal interface ISerializationDelegateProvider
	{
		bool CanHandle(Type type);
		JsonSerializationTypeRegistry.ToJsonDelegate<T> GetEncoder<T>();
		JsonSerializationTypeRegistry.FromJsonDelegate<T> GetDecoder<T>();
	}
}