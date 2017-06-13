using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Allows the user to specify how a property is mapped during serialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class JsonMapToAttribute : Attribute
	{
		///<summary>
		/// Specifies the key in the JSON object which maps to the property to which
		/// this attribute is applied.
		///</summary>
		public string MapToKey { get; }

		/// <summary>
		/// Creates a new instance fo the <see cref="JsonMapToAttribute"/> class.
		/// </summary>
		/// <param name="key">The JSON object key.</param>
		public JsonMapToAttribute(string key)
		{
			MapToKey = key;
		}
	}
}
