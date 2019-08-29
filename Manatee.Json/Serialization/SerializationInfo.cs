using System.Reflection;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Describes a type's member information including the serialized name
	/// and whether the name should be transformed using the serializer's name
	/// transformation logic.
	/// </summary>
	public class SerializationInfo
	{
		/// <summary>
		/// Gets the member information.  Could be a property or a field.
		/// </summary>
		public MemberInfo MemberInfo { get; }
		/// <summary>
		/// Gets the value of the key under which the property should be serialized (the JSON property).
		/// </summary>
		public string SerializationName { get; }
		/// <summary>
		/// Gets whether the name should be transformed by <see cref="JsonSerializerOptions.SerializationNameTransform"/>
		/// or <see cref="JsonSerializerOptions.DeserializationNameTransform"/>.
		/// </summary>
		public bool ShouldTransform { get; }

	    internal SerializationInfo(MemberInfo memberInfo, string serializationName, bool shouldTransform)
		{
			MemberInfo = memberInfo;
			SerializationName = serializationName;
		    ShouldTransform = shouldTransform;
		}

	    /// <summary>Determines whether the specified object is equal to the current object.</summary>
	    /// <param name="obj">The object to compare with the current object.</param>
	    /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
	    public override bool Equals(object obj)
		{
			return obj != null && MemberInfo.Equals(((SerializationInfo) obj).MemberInfo);
		}
	    /// <summary>Serves as the default hash function.</summary>
	    /// <returns>A hash code for the current object.</returns>
	    public override int GetHashCode()
		{
			return MemberInfo.GetHashCode();
		}
	}
}