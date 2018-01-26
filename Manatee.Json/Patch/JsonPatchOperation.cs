using System.ComponentModel.DataAnnotations;

namespace Manatee.Json.Patch
{
	/// <summary>
	/// Defines available operations for JSON Patch actions.
	/// </summary>
	public enum JsonPatchOperation
	{
		/// <summary>
		/// Indicates an addition operation.
		/// </summary>
		[Display(Description = "add")]
		Add,
		/// <summary>
		/// Indicates a removal operation.
		/// </summary>
		[Display(Description = "remove")]
		Remove,
		/// <summary>
		/// Indicates a replacement operation.
		/// </summary>
		[Display(Description = "replace")]
		Replace,
		/// <summary>
		/// Indicates a movement operation.
		/// </summary>
		[Display(Description = "move")]
		Move,
		/// <summary>
		/// Indicates a copy operation.
		/// </summary>
		[Display(Description = "copy")]
		Copy,
		/// <summary>
		/// Indicates a test operation.
		/// </summary>
		[Display(Description = "test")]
		Test
	}
}