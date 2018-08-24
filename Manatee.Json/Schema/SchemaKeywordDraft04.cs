namespace Manatee.Json.Schema
{
	public class SchemaKeywordDraft04 : SchemaKeyword
	{
		public override string Name => "schema";
		public override JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;

		public SchemaKeywordDraft04(string value)
			: base(value) { }
	}
}