using System;
#pragma warning disable 618

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
		/// ```
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
		/// ```
		/// </example>
		[Obsolete("This will be replaced by IgnoreSiblingKeywords")]
		IgnoreSiblingId,
		/// <summary>
		/// Sibling keywords will be ignored. This will process `$ref` according to draft-07 and earlier.
		/// </summary>
		/// <remarks>
		/// This has `$ref` resolution implications around allowing `$id` to change the base URI.
		/// </remarks>
		/// <example>
		/// The `$ref` in the following schema will resolve to <i>http://example.com/document.json</i>
		/// because the `$id` of <i>folder</i> will be ignored.
		/// 
		/// ```
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
		/// ```
		/// </example>
		IgnoreSiblingKeywords = IgnoreSiblingId,
		/// <summary>
		/// Sibling `$id` properties will be processed and will change the
		/// base URI.  This will process `$ref` according to draft 2019-09.
		/// </summary>
		/// <example>
		/// The `$ref` in the following schema will resolve to <i>http://example.com/folder/document.json</i>
		/// because the `$id` of <i>folder</i> will be processed.
		/// 
		/// ```
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
		/// ```
		/// </example>
		[Obsolete("This will be replaced by ProcessSiblingKeywords")]
		ProcessSiblingId,
		/// <summary>
		/// Sibling keywords will be processed. This will process `$ref` *and* sibling keywords according to draft 2019-09.
		/// </summary>
		/// <remarks>
		/// This has `$ref` resolution implications around allowing `$id` to change the base URI.
		/// </remarks>
		/// <example>
		/// The `$ref` in the following schema will resolve to <i>http://example.com/folder/document.json</i>
		/// because the `$id` of <i>folder</i> will be processed.
		/// 
		/// ```
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
		/// ```
		/// </example>
		ProcessSiblingKeywords = ProcessSiblingId
	}
}