/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		PrimitiveMapper.cs
	Namespace:		Manatee.Json.Helpers
	Class Name:		PrimitiveMapper
	Purpose:		Provides type-safe generic casting with additional functionality.

***************************************************************************************/
using System;
using Manatee.Json.Enumerations;
using Manatee.Json.Exceptions;

namespace Manatee.Json.Helpers
{
	internal class PrimitiveMapper : IPrimitiveMapper
	{
		private readonly IObjectCaster _objectCaster;

		public PrimitiveMapper(IObjectCaster objectCaster)
		{
			if (objectCaster == null)
				throw new ArgumentNullException("objectCaster");
			_objectCaster = objectCaster;
		}

		public JsonValue MapToJson<T>(T obj)
		{
			if (obj is string)
				return new JsonValue(_objectCaster.Cast<string>(obj));
			if (obj is bool)
				return new JsonValue(_objectCaster.Cast<bool>(obj));
			if (obj is Enum)
				return new JsonValue(_objectCaster.Cast<int>(obj));
			double result;
			return _objectCaster.TryCast(obj, out result) ? result : JsonValue.Null;
		}

		public T MapFromJson<T>(JsonValue json)
		{
			if (!IsPrimitive(typeof(T)))
			    throw new NotPrimitiveTypeException(typeof(T));
			if (typeof(Enum).IsAssignableFrom(typeof(T)))
				return (T) Enum.ToObject(typeof (T), (int) json.Number);
			var value = GetValue(json);
			T result;
			_objectCaster.TryCast(value, out result);
			return result;
		}

		public bool IsPrimitive(Type type)
		{
			return type.IsPrimitive || (type == typeof (string)) || (typeof(Enum).IsAssignableFrom(type));
		}

		private static object GetValue(JsonValue json)
		{
			switch (json.Type)
			{
				case JsonValueType.Number:
					return json.Number;
				case JsonValueType.String:
					return json.String;
				case JsonValueType.Boolean:
					return json.Boolean;
				case JsonValueType.Null:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
