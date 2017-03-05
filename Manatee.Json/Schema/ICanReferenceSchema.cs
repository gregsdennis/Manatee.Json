namespace Manatee.Json.Schema
{
	internal interface ICanReferenceSchema
	{
		void ResolveReferences(JsonValue root);
	}
}