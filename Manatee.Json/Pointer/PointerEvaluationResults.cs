namespace Manatee.Json.Pointer
{
	/// <summary>
	/// Provides results for a JSON Pointer evaluation.
	/// </summary>
	public class PointerEvaluationResults
	{
		/// <summary>
		/// Gets the referenced value, if found.
		/// </summary>
		public JsonValue? Result { get; }
		/// <summary>
		/// Gets any errors that may have resulted in not finding the referenced value.
		/// </summary>
		public string? Error { get; }

		internal PointerEvaluationResults(JsonValue found)
		{
			Result = found;
		}
		internal PointerEvaluationResults(string error)
		{
			Error = error;
		}
	}
}