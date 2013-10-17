using System;
using System.Reflection;

namespace Manatee.Json.Helpers
{
	internal interface ISerializerCache
	{
		MethodInfo GetSerializeMethod(Type type);
		MethodInfo GetDeserializeMethod(Type type);
	}
}