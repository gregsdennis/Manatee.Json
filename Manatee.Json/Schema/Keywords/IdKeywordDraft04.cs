namespace Manatee.Json.Schema
{
	public class IdKeywordDraft04 : IdKeyword
	{
		public override string Name => "id";
		public override JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;
		public int ValidationSequence => 1;

		public IdKeywordDraft04(string value)
			: base(value) { }
	}
}