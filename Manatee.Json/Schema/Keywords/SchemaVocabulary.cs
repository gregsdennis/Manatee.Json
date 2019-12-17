using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Represents a draft-08 schema vocabulary.
	/// </summary>
	/// <remarks>
	/// Currently a vocabulary is merely a link between the vocabulary ID and the meta-schema ID.
	/// In the future, as vocabularies are better defined, there may be a data format for them.
	/// As the feature develops in JSON Schema, it will be developed within this library.
	/// </remarks>
	[DebuggerDisplay("{Id}")]
	public class SchemaVocabulary : IEquatable<SchemaVocabulary>
	{
		/// <summary>
		/// Gets the vocabulary ID.
		/// </summary>
		public string Id { get; }
		/// <summary>
		/// Gets the ID of the meta-schema associated with the vocabulary.
		/// </summary>
		public string MetaSchemaId { get; }

		internal List<Type> DefinedKeywords { get; } = new List<Type>();

		/// <summary>
		/// Creates a new instance of the <see cref="SchemaVocabulary"/> class.
		/// </summary>
		public SchemaVocabulary(string id, string metaSchemaId)
		{
			Id = id;
			MetaSchemaId = metaSchemaId;
		}
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		internal SchemaVocabulary(string id)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			Id = id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(SchemaVocabulary? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Id, other.Id) && string.Equals(MetaSchemaId, other.MetaSchemaId);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object? obj)
		{
			return Equals(obj as SchemaVocabulary);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (MetaSchemaId != null ? MetaSchemaId.GetHashCode() : 0);
			}
		}
	}
}
