using System;

namespace Manatee.Json.Helpers
{
	internal interface IPrimitiveMapper
	{
		JsonValue MapToJson<T>(T obj);
		T MapFromJson<T>(JsonValue json);
		bool IsPrimitive(Type type);
	}
}