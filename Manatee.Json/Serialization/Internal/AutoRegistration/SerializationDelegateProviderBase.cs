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
	Purpose:		Provides a base implementation for ISerializationDelegateProvider.

***************************************************************************************/
using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal abstract class SerializationDelegateProviderBase : ISerializationDelegateProvider
	{
		private readonly MethodInfo _encodeMethod;
		private readonly MethodInfo _decodeMethod;

		protected SerializationDelegateProviderBase()
		{
			_encodeMethod = GetType().GetMethod("Encode", BindingFlags.NonPublic | BindingFlags.Static);
			_decodeMethod = GetType().GetMethod("Decode", BindingFlags.NonPublic | BindingFlags.Static);
		}

		public abstract bool CanHandle(Type type);
		public JsonSerializationTypeRegistry.ToJsonDelegate<T> GetEncoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var toJson = _encodeMethod;
			if (toJson.IsGenericMethod)
				toJson = toJson.MakeGenericMethod(typeArguments);
			return (JsonSerializationTypeRegistry.ToJsonDelegate<T>) Delegate.CreateDelegate(typeof (JsonSerializationTypeRegistry.ToJsonDelegate<T>), toJson);
		}
		public JsonSerializationTypeRegistry.FromJsonDelegate<T> GetDecoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var fromJson = _decodeMethod;
			if (fromJson.IsGenericMethod)
				fromJson = fromJson.MakeGenericMethod(typeArguments);
			return (JsonSerializationTypeRegistry.FromJsonDelegate<T>) Delegate.CreateDelegate(typeof (JsonSerializationTypeRegistry.FromJsonDelegate<T>), fromJson);
		}

		protected virtual Type[] GetTypeArguments(Type type)
		{
			return type.GetGenericArguments();
		}
	}
}