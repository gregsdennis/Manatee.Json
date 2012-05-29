using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Manatee.Json.Helpers
{
	class PropertyReference
	{
		public PropertyInfo Info { get; set; }
		public object Owner { get; set; }
	}
}
