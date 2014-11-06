/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		DictionarySerializationDelegateProvider.cs
	Namespace:		Manatee.Json.Serialization.Internal.AutoRegistration
	Class Name:		DictionarySerializationDelegateProvider
	Purpose:		Provides delegates for serializing GUIDs.

***************************************************************************************/
using System;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class GuidSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type == typeof(TimeSpan);
		}

		private static JsonValue Encode(Guid guid, JsonSerializer serializer)
		{
			return guid.ToString();
		}
		private static Guid Decode(JsonValue json, JsonSerializer serializer)
		{
			return json.Type == JsonValueType.String ? new Guid(json.String) : default(Guid);
		}
	}
}