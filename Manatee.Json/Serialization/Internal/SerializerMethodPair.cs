using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializerMethodPair
	{
		public MethodInfo Serializer { get; }
		public MethodInfo Deserializer { get; }

		public SerializerMethodPair(Type type)
		{
			Serializer = _GetTypedSerializeMethod(type);
			Deserializer = _GetTypedDeserializeMethod(type);
		}

		private static MethodInfo _GetTypedSerializeMethod(Type type)
		{
			return typeof(JsonSerializer).GetTypeInfo().GetDeclaredMethod("Serialize")
										 .MakeGenericMethod(type);
		}
		private static MethodInfo _GetTypedDeserializeMethod(Type type)
		{
			return typeof(JsonSerializer).GetTypeInfo().GetDeclaredMethod("Deserialize")
										 .MakeGenericMethod(type);
		}
	}
}
