using System;

namespace Manatee.Json.Serialization
{
	///<summary>
	/// Thrown when an abstract or interface type is mapped to another abstract or interface type.
	///</summary>
	public class JsonTypeMapException : Exception
	{
		internal JsonTypeMapException(Type abstractType, Type concreteType)
			: base($"Cannot create map from type '{abstractType}' to type '{concreteType}' because the destination type is either abstract or an interface.") {}
	}

	///<summary>
	/// Thrown when an abstract or interface type is mapped to another abstract or interface type.
	///</summary>
	///<typeparam name="TAbstract">The type being mapped from.</typeparam>
	///<typeparam name="TConcrete">The type being mapped to.</typeparam>
	public class JsonTypeMapException<TAbstract, TConcrete> : JsonTypeMapException
	{
		/// <summary>
		/// Creates a new instance of the <see cref="JsonTypeMapException&lt;TAbstract, TConcrete&gt;"/> object.
		/// </summary>
		internal JsonTypeMapException()
			: base(typeof (TAbstract), typeof (TConcrete)) {}
	}
}
