using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Thrown when <see cref="CustomSerializations.RegisterType&lt;T&gt;(CustomSerializations.ToJsonDelegate&lt;T&gt;, CustomSerializations.FromJsonDelegate&lt;T&gt;)"/>
	/// is passed one method and a null.
	/// </summary>
	public class TypeRegistrationException : Exception
	{
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeRegistrationException"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		internal TypeRegistrationException(Type type)
			: base($"Attempted to register type {type} without supplying both an encoder and decoder.")
		{
			Type = type;
		}
	}
}
