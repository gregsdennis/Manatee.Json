namespace Manatee.Json.Schema
{
	/// <summary>
	/// Determines how `$ref` keywords are resolved when adjacent to an
	/// `$id` keyword.
	/// </summary>
	/// <remarks>
	/// See the specific members for examples.
	/// </remarks>
	public enum RefResolutionStrategy
	{
		/// <summary>
		/// Sibling `$id` properties will be ignored and will not change the
		/// base URI.  This will process `$ref` according to draft-07 and earlier.
		/// </summary>
		/// <example>
		/// The `$ref` in the following schema will resolve to <i>http://example.com/document.json</i>
		/// because the `$id` of <i>folder</i> will be ignored.
		/// 
		/// `
		/// {
		///   "$schema": "http://json-schema.org/draft-08/schema#",
		///   "$id": "http://example.com/root.json"
		///   "properties": {
		///     "prop": {
		///       "$id": "folder"
		///       "$ref": "document.json"
		///     }
		///   }
		/// }
		/// `
		/// </example>
		IgnoreSiblingId,
		/// <summary>
		/// Sibling `$id` properties will be processed and will change the
		/// base URI.  This will process `$ref` according to draft-08.
		/// </summary>
		/// <example>
		/// The `$ref` in the following schema will resolve to <i>http://example.com/folder/document.json</i>
		/// because the `$id` of <i>folder</i> will be processed.
		/// 
		/// `
		/// {
		///   "$schema": "http://json-schema.org/draft-08/schema#",
		///   "$id": "http://example.com/root.json"
		///   "properties": {
		///     "prop": {
		///       "$id": "folder"
		///       "$ref": "document.json"
		///     }
		///   }
		/// }
		/// `
		/// </example>
		ProcessSiblingId
	}
}