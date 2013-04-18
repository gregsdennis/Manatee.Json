using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Json.Exceptions
{
	public class JsonTypeMapException<TAbstract, TConcrete> : Exception
	{
		public JsonTypeMapException()
			: base(string.Format("Cannot create map from type '{0}' to type '{1}' because the destination type is either abstract or an interface.",
								 typeof(TAbstract),
								 typeof(TConcrete))) {}
	}
}
