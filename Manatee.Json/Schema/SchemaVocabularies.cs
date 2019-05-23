namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the official draft-08 vocabularies.
	/// </summary>
	public static class SchemaVocabularies
	{
		/// <summary>
		/// Used for keywords that only exist in schema versions prior to 2019-04.
		/// </summary>
		public static readonly SchemaVocabulary Pre2019_04 = new SchemaVocabulary("pre-draft-08");
		/// <summary>
		/// Identifies the 2019-04 core keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Core = new SchemaVocabulary("https://json-schema.org/2019-04/vocab/core", "https://json-schema.org/2019-04/core");
		/// <summary>
		/// Identifies the 2019-04 applicator keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Applicator = new SchemaVocabulary("https://json-schema.org/2019-04/vocab/applicator", "https://json-schema.org/2019-04/meta/applicator");
		/// <summary>
		/// Identifies the 2019-04 meta-data keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary MetaData = new SchemaVocabulary("https://json-schema.org/2019-04/vocab/meta-data", "https://json-schema.org/2019-04/meta/meta-data");
		/// <summary>
		/// Identifies the 2019-04 validation keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Validation = new SchemaVocabulary("https://json-schema.org/2019-04/vocab/validation", "https://json-schema.org/2019-04/meta/validation");
		/// <summary>
		/// Identifies the 2019-04 format keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Format = new SchemaVocabulary("https://json-schema.org/2019-04/vocab/format", "https://json-schema.org/2019-04/meta/format");
		/// <summary>
		/// Identifies the 2019-04 content keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Content = new SchemaVocabulary("https://json-schema.org/2019-04/vocab/content", "https://json-schema.org/2019-04/meta/content");
	}
}