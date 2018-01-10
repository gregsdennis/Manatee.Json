namespace Manatee.Json.Pointer
{
	public class PointerEvaluationResults
	{
		public JsonValue Result { get; }
		public string Error { get; }

		public PointerEvaluationResults(JsonValue found)
		{
			Result = found;
		}
		public PointerEvaluationResults(string error)
		{
			Error = error;
		}
	}
}