using System;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the <code>definitions</code> JSON Schema keyword.
	/// </summary>
	public class DefinitionsKeyword : DefsKeyword, IEquatable<DefinitionsKeyword>
	{
		/// <summary>
		/// Gets the name of the keyword.
		/// </summary>
		public override string Name => "definitions";

		/// <summary>
		/// Gets the versions (drafts) of JSON Schema which support this keyword.
		/// </summary>
		public override JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04 |
		                                                               JsonSchemaVersion.Draft06 |
		                                                               JsonSchemaVersion.Draft07;

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(DefinitionsKeyword other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return base.Equals(other);
		}
		/// <summary>Determines whether the specified object is equal to the current object.</summary>
		/// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as DefinitionsKeyword);
		}
		/// <summary>Serves as the default hash function. </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}