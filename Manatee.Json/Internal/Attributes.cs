using System;

namespace Manatee.Json.Internal
{
#if !NET45
	//[CompilerAttributes.GeneratesError("This constructor is provided for deserialization purposes only.  Please use the parameterized one instead.")]
#endif
	public class DeserializationUseOnlyAttribute : Attribute
	{
	}
}
