using System.Collections.Generic;

namespace Manatee.Json.Tests.Test_References
{
	internal class Issue126
	{
		public int X { get; set; }
		public Dictionary<string, bool> Y { get; set; }
		public Dictionary<string, Issue126> Z { get; set; }
	}
}
