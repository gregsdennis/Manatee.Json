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
 
	File Name:		ObjectCasterMethodPair.cs
	Namespace:		Manatee.Json.Cache
	Class Name:		ObjectCasterMethodPair
	Purpose:		Represents a typed pair of ObjectCaster methods.

***************************************************************************************/
using System;
using System.Reflection;
using Manatee.Json.Helpers;

namespace Manatee.Json.Helpers
{
	internal class ObjectCasterMethodPair
	{
		public MethodInfo Caster { get; private set; }
		public MethodInfo TryCaster { get; private set; }

		public ObjectCasterMethodPair(Type type)
		{
			Caster = GetTypedSerializeMethod(type);
			TryCaster = GetTypedDeserializeMethod(type);
		}

		private static MethodInfo GetTypedSerializeMethod(Type type)
		{
			return typeof(ObjectCaster).GetMethod("Cast").MakeGenericMethod(type);
		}
		private static MethodInfo GetTypedDeserializeMethod(Type type)
		{
			return typeof(ObjectCaster).GetMethod("TryCast").MakeGenericMethod(type);
		}
	}
}
