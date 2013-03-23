using System;
using System.Reflection;

namespace Manatee.Json.Helpers
{
	internal interface ISerializerCache
	{
		MethodInfo GetSerializer(Type type);
		MethodInfo GetDeserializer(Type type);
	}
}