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
 
	File Name:		JsonSerializerOptions.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonSerializerOptions
	Purpose:		Default options used by the serializer.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Represents a set of behavior options for the <see cref="JsonSerializer"/> object.
	/// </summary>
	public class JsonSerializerOptions
	{
		/// <summary>
		/// Default options used by the serializer.
		/// </summary>
		public static readonly JsonSerializerOptions Default;
		/// <summary>
		/// Gets and sets whether the serializer encodes default values for properties.
		/// </summary>
		/// <remarks>
		/// Setting to 'true' may significantly increase the size of the JSON structure.
		/// </remarks>
		public bool EncodeDefaultValues { get; set; }
		/// <summary>
		/// Gets and sets the behavior of the deserializer when a JSON structure is passed which
		/// contains invalid property keys.
		/// </summary>
		public InvalidPropertyKeyBehavior InvalidPropertyKeyBehavior { get; set; }
		/// <summary>
		/// Gets and sets the format for <see cref="DateTime"/> serialization using the default serializer methods.
		/// </summary>
		/// <remarks>
		/// If the <see cref="JsonSerializationTypeRegistry"/> entry for DateTime has been changed to custom
		/// methods, this property will have no effect.
		/// </remarks>
		public DateTimeSerializationFormat DateTimeSerializationFormat { get; set; }
		/// <summary>
		/// Gets and sets a custom serialization format for <see cref="DateTime"/>.
		/// </summary>
		public string CustomDateTimeSerializationFormat { get; set; }
		/// <summary>
		/// Gets and sets the format for enumeration serialization using the default serializer methods.
		/// </summary>
		/// <remarks>
		/// If an entry has been made in <see cref="JsonSerializationTypeRegistry"/> for the specific type,
		/// this property will have no effect.
		/// </remarks>
		public EnumSerializationFormat EnumSerializationFormat { get; set; }
		/// <summary>
		/// Gets and sets a separator to be used when serializing enumerations marked with the <see cref="FlagsAttribute"/>.
		/// </summary>
		public string FlagsEnumSeparator { get; set; }
		/// <summary>
		/// Gets and sets whether the serializer considers case for properties while deserializing.
		/// </summary>
		/// <remarks>
		/// This only affects automatic serialization.
		/// </remarks>
		public bool CaseSensitiveDeserialization { get; set; }
		/// <summary>
		/// Gets and sets whether the serializer always includes the type name while serializing.
		/// </summary>
		/// <remarks>
		/// This only affects automatic serialization.
		/// </remarks>
		public TypeNameSerializationBehavior TypeNameSerializationBehavior { get; set; }
		/// <summary>
		/// Gets and sets which properties are automatically included while serializing.
		/// </summary>
		public PropertySelectionStrategy PropertySelectionStrategy { get; set; }
		/// <summary>
		/// Gets and sets an <see cref="IResolver"/> implementation for instantiating objects while deserializing.
		/// </summary>
		public IResolver Resolver { get; set; }
		/// <summary>
		/// Gets and sets whether public fields should be serialized during autoserialization.
		/// </summary>
		public bool AutoSerializeFields { get; set; }

		internal bool IncludeContentSample { get; set; }

		static JsonSerializerOptions()
		{
			Default = new JsonSerializerOptions
				{
					EncodeDefaultValues = false,
					InvalidPropertyKeyBehavior = InvalidPropertyKeyBehavior.DoNothing,
					EnumSerializationFormat = EnumSerializationFormat.AsInteger
				};
		}
		/// <summary>
		/// Creates a new instance of <see cref="JsonSerializerOptions"/> with default options.
		/// </summary>
		public JsonSerializerOptions()
		{
			PropertySelectionStrategy = PropertySelectionStrategy.ReadWriteOnly;
		}
		/// <summary>
		/// Creates a new instance of <see cref="JsonSerializerOptions"/> by copying an existing
		/// <see cref="JsonSerializerOptions"/> instance.
		/// </summary>
		/// <param name="options">The <see cref="JsonSerializerOptions"/> instance to copy.</param>
		public JsonSerializerOptions(JsonSerializerOptions options)
			: this()
		{
			EncodeDefaultValues = options.EncodeDefaultValues;
			InvalidPropertyKeyBehavior = options.InvalidPropertyKeyBehavior;
			DateTimeSerializationFormat = options.DateTimeSerializationFormat;
			EnumSerializationFormat = options.EnumSerializationFormat;
		}
	}
}
