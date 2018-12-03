namespace Manatee.Json.Schema
{
	public static partial class MetaSchemas
	{
		public static readonly JsonSchema Draft08 =
			new JsonSchema()
				.Schema("http://json-schema.org/draft-08/schema#")
				.Id("http://json-schema.org/draft-08/schema#");
	}
}