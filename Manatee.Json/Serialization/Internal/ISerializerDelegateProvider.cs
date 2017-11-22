using System;

namespace Manatee.Json.Serialization.Internal
{
	internal interface ISerializationDelegateProvider
	{
		bool CanHandle(Type type);
		CustomSerializations.ToJsonDelegate<T> GetEncoder<T>();
		CustomSerializations.FromJsonDelegate<T> GetDecoder<T>();
	}
}