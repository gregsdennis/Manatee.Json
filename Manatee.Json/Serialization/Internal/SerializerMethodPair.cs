using System;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializerMethodPair
	{
		public MethodInfo Serializer { get; private set; }
		public MethodInfo Deserializer { get; private set; }

		public SerializerMethodPair(Type type)
		{
			Serializer = GetTypedSerializeMethod(type);
			Deserializer = GetTypedDeserializeMethod(type);
		}

		private static MethodInfo GetTypedSerializeMethod(Type type)
		{
			return typeof(JsonSerializer).GetMethod("Serialize")
										 .MakeGenericMethod(type);
		}
		private static MethodInfo GetTypedDeserializeMethod(Type type)
		{
			return typeof(JsonSerializer).GetMethod("Deserialize")
										 .MakeGenericMethod(type);
		}
	}
}
