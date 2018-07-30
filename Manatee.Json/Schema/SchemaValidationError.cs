using System;
using System.Linq;
using Manatee.Json.Pointer;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Represents a single schema validation error.
	/// </summary>
	public class SchemaValidationError : IEquatable<SchemaValidationError>
	{
		/// <summary>
		/// A pointer to the schema property which failed validation.
		/// </summary>
		public JsonPointer Property { get; private set; }
		/// <summary>
		/// A message indicating the failure.
		/// </summary>
		public string Message { get; }

		internal SchemaValidationError(string propertyName, string message)
		{
			if (propertyName != null)
				Property = new JsonPointer(propertyName);
			Message = message;
		}

		internal SchemaValidationError PrependPropertySegment(string parentSegment)
		{
			if (Property == null)
				Property = new JsonPointer(parentSegment);
			else
				Property.Insert(0, parentSegment);
			return this;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return Property == null
				? Message
				: $"Property: {Property} - {Message}";
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(SchemaValidationError other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Property == null && other.Property == null) ||
			       (Property != null && Property.SequenceEqual(other.Property) && string.Equals(Message, other.Message));
		}
		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
		/// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as SchemaValidationError);
		}
		/// <summary>Serves as a hash function for a particular type. </summary>
		/// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((Property?.ToString().GetHashCode() ?? 0)*397) ^ (Message?.GetHashCode() ?? 0);
			}
		}
	}
}