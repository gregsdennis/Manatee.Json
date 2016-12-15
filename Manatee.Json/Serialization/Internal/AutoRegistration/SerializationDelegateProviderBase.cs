﻿/***************************************************************************************

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
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.AutoRegistration
{
	internal abstract class SerializationDelegateProviderBase : ISerializationDelegateProvider
	{
		private readonly MethodInfo _encodeMethod;
		private readonly MethodInfo _decodeMethod;

		protected SerializationDelegateProviderBase()
		{
#if !IOS
			_encodeMethod = GetType().GetMethod("Encode", BindingFlags.NonPublic | BindingFlags.Static);
			_decodeMethod = GetType().GetMethod("Decode", BindingFlags.NonPublic | BindingFlags.Static);
#else
			_encodeMethod = GetType().TypeInfo().GetDeclaredMethod("Encode");
			_decodeMethod = GetType().TypeInfo().GetDeclaredMethod("Decode");
#endif
		}

		public abstract bool CanHandle(Type type);
		public JsonSerializationTypeRegistry.ToJsonDelegate<T> GetEncoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var toJson = _encodeMethod;
			if (toJson.IsGenericMethod)
				toJson = toJson.MakeGenericMethod(typeArguments);
#if IOS || CORE
			return (JsonSerializationTypeRegistry.ToJsonDelegate<T>) toJson.CreateDelegate(typeof (JsonSerializationTypeRegistry.ToJsonDelegate<T>), toJson);
#else
			return (JsonSerializationTypeRegistry.ToJsonDelegate<T>) Delegate.CreateDelegate(typeof (JsonSerializationTypeRegistry.ToJsonDelegate<T>), toJson);
#endif
		}
		public JsonSerializationTypeRegistry.FromJsonDelegate<T> GetDecoder<T>()
		{
			var typeArguments = GetTypeArguments(typeof (T));
			var fromJson = _decodeMethod;
			if (fromJson.IsGenericMethod)
				fromJson = fromJson.MakeGenericMethod(typeArguments);
#if IOS || CORE
			return (JsonSerializationTypeRegistry.FromJsonDelegate<T>) fromJson.CreateDelegate(typeof(JsonSerializationTypeRegistry.FromJsonDelegate<T>), fromJson);
#else
			return (JsonSerializationTypeRegistry.FromJsonDelegate<T>) Delegate.CreateDelegate(typeof (JsonSerializationTypeRegistry.FromJsonDelegate<T>), fromJson);
#endif
		}

		protected virtual Type[] GetTypeArguments(Type type)
		{
			return type.GetTypeArguments();
		}
	}
}