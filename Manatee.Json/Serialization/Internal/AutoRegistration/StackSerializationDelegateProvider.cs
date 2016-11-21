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
 
	File Name:		DictionarySerializationDelegateProvider.cs
	Namespace:		Manatee.Json.Serialization.Internal.AutoRegistration
	Class Name:		DictionarySerializationDelegateProvider
	Purpose:		Provides delegates for serializing Stack types.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal class StackSerializationDelegateProvider : SerializationDelegateProviderBase
	{
		public override bool CanHandle(Type type)
		{
			return type.TypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Stack<>);
		}

		private static JsonValue Encode<T>(Stack<T> stack, JsonSerializer serializer)
		{
			var array = new JsonArray();
			for (int i = 0; i < stack.Count; i++)
			{
				array.Add(serializer.Serialize(stack.ElementAt(i)));
			}
			return array;
		}
		private static Stack<T> Decode<T>(JsonValue json, JsonSerializer serializer)
		{
			var stack = new Stack<T>();
			for (int i = 0; i < json.Array.Count; i++)
			{
				stack.Push(serializer.Deserialize<T>(json.Array[i]));
			}
			return stack;
		}
	}
}