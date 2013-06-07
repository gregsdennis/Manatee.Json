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
 
	File Name:		SerializerFactory.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		SerializerFactory
	Purpose:		Manages ISerializer implementations for use by the JsonSerializer.

***************************************************************************************/

using System;
using System.Collections.Generic;
using Manatee.Json.Enumerations;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializerFactory : ISerializerFactory
	{
		private static readonly AutoSerializer AutoSerializer;
		private static readonly BooleanSerializer BooleanSerializer;
		private static readonly EnumNameSerializer EnumNameSerializer;
		private static readonly EnumValueSerializer EnumValueSerializer;
		private static readonly JsonCompatibleSerializer JsonCompatibleSerializer;
		private static readonly NumericSerializer NumericSerializer;
		private static readonly RegisteredObjectSerializer RegisteredObjectSerializer;
		private static readonly StringSerializer StringSerializer;
		private static readonly Dictionary<Type, ISerializer> Library;

		static SerializerFactory()
		{
			AutoSerializer = new AutoSerializer();
			BooleanSerializer = new BooleanSerializer();
			EnumNameSerializer = new EnumNameSerializer();
			EnumValueSerializer = new EnumValueSerializer();
			JsonCompatibleSerializer = new JsonCompatibleSerializer();
			NumericSerializer = new NumericSerializer();
			RegisteredObjectSerializer = new RegisteredObjectSerializer();
			StringSerializer = new StringSerializer();
			Library = new Dictionary<Type, ISerializer>
				{
					{typeof (sbyte), NumericSerializer},
					{typeof (byte), NumericSerializer},
					{typeof (char), NumericSerializer},
					{typeof (short), NumericSerializer},
					{typeof (ushort), NumericSerializer},
					{typeof (int), NumericSerializer},
					{typeof (uint), NumericSerializer},
					{typeof (long), NumericSerializer},
					{typeof (ulong), NumericSerializer},
					{typeof (float), NumericSerializer},
					{typeof (double), NumericSerializer},
					{typeof (decimal), NumericSerializer},
					{typeof (bool), BooleanSerializer},
					{typeof (string), StringSerializer},
				};
		}

		public ISerializer GetSerializer<T>(JsonSerializerOptions options)
		{
			var type = typeof (T);
			if (JsonSerializationTypeRegistry.IsRegistered(type))
				return BuildSerializer(RegisteredObjectSerializer);
			if (typeof(IJsonCompatible).IsAssignableFrom(type))
				return BuildSerializer(JsonCompatibleSerializer);
			if (typeof(Enum).IsAssignableFrom(type))
			{
				switch (options.EnumSerializationFormat)
				{
					case EnumSerializationFormat.AsInteger:
						return BuildSerializer(EnumValueSerializer);
					case EnumSerializationFormat.AsName:
						return BuildSerializer(EnumNameSerializer);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			if (Library.ContainsKey(type))
				return BuildSerializer(Library[type]);
			return BuildSerializer(AutoSerializer);
		}
		public ITypeSerializer GetTypeSerializer<T>(JsonSerializerOptions options)
		{
			return AutoSerializer;
		}

		private static ISerializer BuildSerializer(ISerializer innerSerializer)
		{
			return new DefaultValueSerializer(new ReferencingSerializer(innerSerializer));
		}
	}
}