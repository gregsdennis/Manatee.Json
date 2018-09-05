using System.Collections.Generic;

namespace Manatee.Json.Schema
{
	public class SchemaValidationContext
	{
		public JsonSchema Local { get; set; }
		public JsonSchema Root { get; set; }
		public JsonValue Instance { get; set; }
		public List<string> EvaluatedPropertyNames { get; set; }
	}
}