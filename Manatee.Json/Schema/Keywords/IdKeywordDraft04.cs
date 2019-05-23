using System;
using System.Diagnostics;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the <code>id</code> JSON Schema keyword for draft-04.
	/// </summary>
	[DebuggerDisplay("Name={Name} Value={Value}")]
	public class IdKeywordDraft04 : IdKeyword, IEquatable<IdKeywordDraft04>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public override string Name => "id";
		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public override JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;
		/// <summary>
		/// Gets the vocabulary that defines this keyword.
		/// </summary>
		public override SchemaVocabulary Vocabulary => SchemaVocabularies.Pre2019_04;

		/// <summary>
		/// Used for deserialization.
		/// </summary>
		[DeserializationUseOnly]
		public IdKeywordDraft04() { }
		/// <summary>
		/// Creates an instance of the <see cref="IdKeywordDraft04"/>.
		/// </summary>
		public IdKeywordDraft04(string value)
			: base(value) { }

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IdKeywordDraft04 other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Value == other.Value;
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as IdKeywordDraft04);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}
	}
}