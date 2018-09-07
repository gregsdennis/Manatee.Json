using System;
using System.Diagnostics;

namespace Manatee.Json.Schema
{
	[DebuggerDisplay("Name={Name}")]
	public class IdKeywordDraft04 : IdKeyword, IEquatable<IdKeywordDraft04>
	{
		public override string Name => "id";
		public override JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;

		public IdKeywordDraft04() { }
		public IdKeywordDraft04(string value)
			: base(value) { }

		public bool Equals(IdKeywordDraft04 other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Value == other.Value;
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as IdKeywordDraft04);
		}
		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}
	}
}