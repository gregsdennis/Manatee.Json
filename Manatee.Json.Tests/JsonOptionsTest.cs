using System;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonOptionsTest
	{
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
