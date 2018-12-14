namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the official draft-08 vocabularies.
	/// </summary>
	public static class SchemaVocabularies
	{
		/// <summary>
		/// Used for keywords that only exist in schema versions prior to draft-08.
		/// </summary>
		public static readonly SchemaVocabulary PreDraft08 = new SchemaVocabulary("pre-draft-08");
		/// <summary>
		/// Identifies the Draft-08 core keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Core = new SchemaVocabulary("https://json-schema.org/draft-08/vocabularies/core", "https://json-schema.org/draft-08/core");
		/// <summary>
		/// Identifies the Draft-08 applicator keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Applicator = new SchemaVocabulary("https://json-schema.org/draft-08/vocabularies/applicator", "https://json-schema.org/draft-08/applicator");
		/// <summary>
		/// Identifies the Draft-08 annotation keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Annotation = new SchemaVocabulary("https://json-schema.org/draft-08/vocabularies/annotation", "https://json-schema.org/draft-08/annotation");
		/// <summary>
		/// Identifies the Draft-08 assertion keywords vocabulary.
		/// </summary>
		public static readonly SchemaVocabulary Assertion = new SchemaVocabulary("https://json-schema.org/draft-08/vocabularies/assertion", "https://json-schema.org/draft-08/assertion");
	}
}