#if IOS || CORE
using System;

namespace Manatee.Json
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class DescriptionAttribute : Attribute
	{
		/// <summary>
		/// Gets the description.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="DescriptionAttribute"/> class.
		/// </summary>
		public DescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}
#endif
