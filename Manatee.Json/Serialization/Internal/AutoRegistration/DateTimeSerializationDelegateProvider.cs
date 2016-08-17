/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		DateTimeSerializationDelegateProvider.cs
	Namespace:		Manatee.Json.Serialization.Internal.AutoRegistration
	Class Name:		DateTimeSerializationDelegateProvider
	Purpose:		Provides delegates for serializing DateTime.

***************************************************************************************/
using System;
using System.Globalization;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class DateTimeSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type == typeof(DateTime);
		}

		private static JsonValue Encode(DateTime dt, JsonSerializer serializer)
		{
			if (serializer.Options == null)
				return dt.ToString();
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return dt.ToString("s");
				case DateTimeSerializationFormat.JavaConstructor:
					return $"/Date({dt.Ticks/TimeSpan.TicksPerMillisecond})/";
				case DateTimeSerializationFormat.Milliseconds:
					return dt.Ticks / TimeSpan.TicksPerMillisecond;
				case DateTimeSerializationFormat.Custom:
					return dt.ToString(serializer.Options.CustomDateTimeSerializationFormat);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		private static DateTime Decode(JsonValue json, JsonSerializer serializer)
		{
			if (serializer.Options == null)
				return DateTime.Parse(json.String);
			switch (serializer.Options.DateTimeSerializationFormat)
			{
				case DateTimeSerializationFormat.Iso8601:
					return DateTime.Parse(json.String);
				case DateTimeSerializationFormat.JavaConstructor:
					return new DateTime(long.Parse(json.String.Substring(6, json.String.Length - 8)) * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Milliseconds:
					return new DateTime((long)json.Number * TimeSpan.TicksPerMillisecond);
				case DateTimeSerializationFormat.Custom:
					return DateTime.ParseExact(json.String, serializer.Options.CustomDateTimeSerializationFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}