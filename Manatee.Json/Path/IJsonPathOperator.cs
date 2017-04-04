namespace Manatee.Json.Path
{
	internal interface IJsonPathOperator
	{
		JsonArray Evaluate(JsonArray json, JsonValue root);
	}
}