using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Applied to properties to indicate that they are not to be serialized.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class JsonIgnoreAttribute : Attribute
	{
	}
}
