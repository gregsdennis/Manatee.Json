namespace Manatee.Json.Patch
{
    public class JsonPatchResult
    {
        public JsonValue Patched { get; }
        public bool Success => Error == null;
        public string Error { get; }

        public JsonPatchResult(JsonValue patched, string error = null)
        {
            Patched = patched;
            Error = error;
        }
    }
}