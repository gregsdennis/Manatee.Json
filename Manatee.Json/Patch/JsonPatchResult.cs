namespace Manatee.Json.Patch
{
	/// <summary>
	/// Provides the results of a JSON Patch application.
	/// </summary>
	public class JsonPatchResult
	{
		/// <summary>
		/// The resulting document, if the patch was successful.
		/// </summary>
		public JsonValue Patched { get; }
		/// <summary>
		/// Gets whether the patch was successful.
		/// </summary>
		public bool Success => Error == null;
		/// <summary>
		/// Gets any errors that have occurred during a patch.
		/// </summary>
		public string Error { get; }

		internal JsonPatchResult(JsonValue patched, string error = null)
		{
			Patched = patched;
			Error = error;
		}
	}
}