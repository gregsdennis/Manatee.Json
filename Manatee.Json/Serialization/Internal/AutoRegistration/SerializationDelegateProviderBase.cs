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
#if !IOS
			_encodeMethod = GetType().GetMethod("Encode", BindingFlags.NonPublic | BindingFlags.Static);
			_decodeMethod = GetType().GetMethod("Decode", BindingFlags.NonPublic | BindingFlags.Static);
#else
			_encodeMethod = GetType().TypeInfo().GetDeclaredMethod("Encode");
			_decodeMethod = GetType().TypeInfo().GetDeclaredMethod("Decode");
#endif
		}

		public abstract bool CanHandle(Type type);
		public JsonSerializationTypeRegistry.ToJsonDelegate<T> GetEncoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var toJson = _encodeMethod;
			if (toJson.IsGenericMethod)
				toJson = toJson.MakeGenericMethod(typeArguments);
#if IOS || CORE
			return (JsonSerializationTypeRegistry.ToJsonDelegate<T>) toJson.CreateDelegate(typeof (JsonSerializationTypeRegistry.ToJsonDelegate<T>), toJson);
#else
			return (JsonSerializationTypeRegistry.ToJsonDelegate<T>) Delegate.CreateDelegate(typeof (JsonSerializationTypeRegistry.ToJsonDelegate<T>), toJson);
#endif
		}
		public JsonSerializationTypeRegistry.FromJsonDelegate<T> GetDecoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var fromJson = _decodeMethod;
			if (fromJson.IsGenericMethod)
				fromJson = fromJson.MakeGenericMethod(typeArguments);
#if IOS || CORE
			return (JsonSerializationTypeRegistry.FromJsonDelegate<T>) fromJson.CreateDelegate(typeof(JsonSerializationTypeRegistry.FromJsonDelegate<T>), fromJson);
#else
			return (JsonSerializationTypeRegistry.FromJsonDelegate<T>) Delegate.CreateDelegate(typeof (JsonSerializationTypeRegistry.FromJsonDelegate<T>), fromJson);
#endif
		}

		protected virtual Type[] GetTypeArguments(Type type)
		{
			return type.GetTypeArguments();
		}
	}
}