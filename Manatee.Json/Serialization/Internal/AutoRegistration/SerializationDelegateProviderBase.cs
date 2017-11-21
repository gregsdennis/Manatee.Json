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
			_encodeMethod = GetType().GetTypeInfo().GetDeclaredMethod("_Encode");
			_decodeMethod = GetType().GetTypeInfo().GetDeclaredMethod("_Decode");
		}

		public abstract bool CanHandle(Type type);
		public CustomSerializations.ToJsonDelegate<T> GetEncoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var toJson = _encodeMethod;
			if (toJson.IsGenericMethod)
				toJson = toJson.MakeGenericMethod(typeArguments);
			return (CustomSerializations.ToJsonDelegate<T>) toJson.CreateDelegate(typeof (CustomSerializations.ToJsonDelegate<T>), null);
		}
		public CustomSerializations.FromJsonDelegate<T> GetDecoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var fromJson = _decodeMethod;
			if (fromJson.IsGenericMethod)
				fromJson = fromJson.MakeGenericMethod(typeArguments);
			return (CustomSerializations.FromJsonDelegate<T>) fromJson.CreateDelegate(typeof(CustomSerializations.FromJsonDelegate<T>), null);
		}

		protected virtual Type[] GetTypeArguments(Type type)
		{
			return type.GetTypeArguments();
		}
	}
}