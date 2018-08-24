using System;

namespace Manatee.Json.Schema
{
	[Flags]
	public enum JsonSchemaVersion
	{
		None = 0,
		Draft04 = 1 << 0,
		Draft06 = 1 << 1,
		Draft07 = 1 << 2,
		Draft08 = 1 << 3,
		All = Draft04 | Draft06 | Draft07 | Draft08
	}
}