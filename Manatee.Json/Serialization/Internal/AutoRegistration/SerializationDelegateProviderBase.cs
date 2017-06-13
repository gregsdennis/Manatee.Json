using System;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal abstract class SerializationDelegateProviderBase : ISerializationDelegateProvider
	{
		private readonly MethodInfo _encodeMethod;
		private readonly MethodInfo _decodeMethod;

		protected SerializationDelegateProviderBase()
		{
			_encodeMethod = GetType().TypeInfo().GetDeclaredMethod("Encode");
			_decodeMethod = GetType().TypeInfo().GetDeclaredMethod("Decode");
		}

		public abstract bool CanHandle(Type type);
		public JsonSerializationTypeRegistry.ToJsonDelegate<T> GetEncoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var toJson = _encodeMethod;
			if (toJson.IsGenericMethod)
				toJson = toJson.MakeGenericMethod(typeArguments);
			return (JsonSerializationTypeRegistry.ToJsonDelegate<T>) toJson.CreateDelegate(typeof (JsonSerializationTypeRegistry.ToJsonDelegate<T>), toJson);
		}
		public JsonSerializationTypeRegistry.FromJsonDelegate<T> GetDecoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var fromJson = _decodeMethod;
			if (fromJson.IsGenericMethod)
				fromJson = fromJson.MakeGenericMethod(typeArguments);
			return (JsonSerializationTypeRegistry.FromJsonDelegate<T>) fromJson.CreateDelegate(typeof(JsonSerializationTypeRegistry.FromJsonDelegate<T>), fromJson);
		}

		protected virtual Type[] GetTypeArguments(Type type)
		{
			return type.GetTypeArguments();
		}
	}
}