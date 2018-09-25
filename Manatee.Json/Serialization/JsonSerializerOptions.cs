using System;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Represents a set of behavior options for the <see cref="JsonSerializer"/> object.
	/// </summary>
	public class JsonSerializerOptions
	{
		private static readonly IResolver _defaultResolver = new ConstructorResolver();
		private IResolver _resolver;
	    private Func<string, string> _serializationNameTransform;
	    private Func<string, string> _deserializationNameTransform;

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
		/// If a custom serializer for DateTime has been registered, this property will have no effect.
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
		/// If a custom serializer for DateTime has been registered, this property will have no effect.
		/// </remarks>
		public EnumSerializationFormat EnumSerializationFormat { get; set; }
		/// <summary>
		/// Gets and sets a separator to be used when serializing enumerations marked with the <see cref="FlagsAttribute"/>.
		/// </summary>
		public string FlagsEnumSeparator { get; set; } = ",";
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
		public IResolver Resolver
		{
			get { return _resolver ?? (_resolver = _defaultResolver); }
			set { _resolver = value; }
		}
		/// <summary>
		/// Gets and sets whether public fields should be serialized during autoserialization.
		/// </summary>
		public bool AutoSerializeFields { get; set; }
	    /// <summary>
	    /// Gets and sets a transformation function for property names during serialization.  Default is no transformation.
	    /// </summary>
	    public Func<string, string> SerializationNameTransform
	    {
	        get { return _serializationNameTransform ?? (_serializationNameTransform = s => s); }
	        set { _serializationNameTransform = value; }
	    }
	    /// <summary>
	    /// Gets and sets a transformation function for property names during deserialization.  Default is no transformation.
	    /// </summary>
	    public Func<string, string> DeserializationNameTransform
	    {
	        get { return _deserializationNameTransform ?? (_deserializationNameTransform = s => s); }
	        set { _deserializationNameTransform = value; }
	    }
		/// <summary>
		/// Gets and sets whether the serializer will serialize only the properties defined by the
		/// type given as the generic parameter.
		/// </summary>
		public bool OnlyExplicitProperties { get; set; }

	    internal bool IncludeContentSample { get; set; }

		static JsonSerializerOptions()
		{
			Default = new JsonSerializerOptions
				{
					EncodeDefaultValues = false,
					InvalidPropertyKeyBehavior = InvalidPropertyKeyBehavior.DoNothing,
					EnumSerializationFormat = EnumSerializationFormat.AsName
				};
		}
		/// <summary>
		/// Creates a new instance of <see cref="JsonSerializerOptions"/> with default options.
		/// </summary>
		public JsonSerializerOptions()
		{
			PropertySelectionStrategy = PropertySelectionStrategy.ReadWriteOnly;
			TypeNameSerializationBehavior = TypeNameSerializationBehavior.OnlyForAbstractions;
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
			CustomDateTimeSerializationFormat = options.CustomDateTimeSerializationFormat;
			EnumSerializationFormat = options.EnumSerializationFormat;
			FlagsEnumSeparator = options.FlagsEnumSeparator;
			CaseSensitiveDeserialization = options.CaseSensitiveDeserialization;
			TypeNameSerializationBehavior = options.TypeNameSerializationBehavior;
			PropertySelectionStrategy = options.PropertySelectionStrategy;
			Resolver = options.Resolver;
			AutoSerializeFields = options.AutoSerializeFields;
			SerializationNameTransform = options.SerializationNameTransform;
			DeserializationNameTransform = options.DeserializationNameTransform;
			OnlyExplicitProperties = options.OnlyExplicitProperties;
			IncludeContentSample = options.IncludeContentSample;
		}
	}
}
