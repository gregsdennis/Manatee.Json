namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the official draft-08 vocabularies.
	/// </summary>
	public static class SchemaVocabularies
	{
		/// <summary>
		/// Used for keywords that only exist in schema versions prior to 2019-09.
		/// </summary>
		public static readonly SchemaVocabulary None = new SchemaVocabulary("none");
		/// <summary>
		/// Identifies the 2019-09 core keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Core = new SchemaVocabulary("https://json-schema.org/draft/2019-09/vocab/core", "https://json-schema.org/draft/2019-09/meta/core");
		/// <summary>
		/// Identifies the 2019-09 applicator keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Applicator = new SchemaVocabulary("https://json-schema.org/draft/2019-09/vocab/applicator", "https://json-schema.org/draft/2019-09/meta/applicator");
		/// <summary>
		/// Identifies the 2019-09 meta-data keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary MetaData = new SchemaVocabulary("https://json-schema.org/draft/2019-09/vocab/meta-data", "https://json-schema.org/draft/2019-09/meta/meta-data");
		/// <summary>
		/// Identifies the 2019-09 validation keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Validation = new SchemaVocabulary("https://json-schema.org/draft/2019-09/vocab/validation", "https://json-schema.org/draft/2019-09/meta/validation");
		/// <summary>
		/// Identifies the 2019-09 format keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Format = new SchemaVocabulary("https://json-schema.org/draft/2019-09/vocab/format", "https://json-schema.org/draft/2019-09/meta/format");
		/// <summary>
		/// Identifies the 2019-09 content keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Content = new SchemaVocabulary("https://json-schema.org/draft/2019-09/vocab/content", "https://json-schema.org/draft/2019-09/meta/content");
	}
}