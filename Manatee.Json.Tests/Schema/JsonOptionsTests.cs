using System;
using NUnit.Framework;

namespace Manatee.Json.Tests.Schema
{
	[TestFixture]
	public class JsonOptionsTests
	{
		#region Equality

		[Test]
		public void DotNetNullEquality()
		{
			JsonOptions.NullEqualityBehavior = NullEqualityBehavior.UseDotNetNull;

			JsonValue dotNetNull = null;
			JsonValue jsonNull = JsonValue.Null;

			Assert.IsFalse(jsonNull == dotNetNull);

			JsonOptions.NullEqualityBehavior = NullEqualityBehavior.UseJsonNull;
		}
		[Test]
		public void JsonNullEquality()
		{
			JsonValue dotNetNull = null;
			JsonValue jsonNull = JsonValue.Null;

			Assert.True(jsonNull == dotNetNull);
		}

		#endregion

		#region DuplicateKeyBehavior

		[Test]
		public void DuplicateKeyThrows()
		{
			Assert.Throws<ArgumentException>(() =>
				{
					var text = "{\"key\":1, \"key\":false}";
					var json = JsonValue.Parse(text);
				});
		}
		[Test]
		public void DuplicateKeyOverwrites()
		{
			JsonOptions.DuplicateKeyBehavior = DuplicateKeyBehavior.Overwrite;

			var text = "{\"key\":1, \"key\":false}";
			var json = JsonValue.Parse(text);

			JsonOptions.DuplicateKeyBehavior = DuplicateKeyBehavior.Throw;
		}

		#endregion
	}
}
