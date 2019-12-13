using System.Collections.Generic;

namespace Manatee.Json.Tests.Test_References
{
	internal class ReadOnlyPreinitializedObject
	{
		public List<string> List { get; } = new List<string>();
	}
}