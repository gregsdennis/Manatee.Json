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

			try
			{
				var text = "{\"key\":1, \"key\":false}";
				var json = JsonValue.Parse(text);
			}
			finally
			{
				JsonOptions.DuplicateKeyBehavior = DuplicateKeyBehavior.Throw;
			}
		}

		#endregion

		#region IncorrectTypeAccess

		[Test]
		public void IncorrectAccessThrows_Object()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					JsonValue json = false;
					var test = json.Object;
				});
		}
		[Test]
		public void IncorrectAccessThrows_Array()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					JsonValue json = false;
					var test = json.Array;
				});
		}
		[Test]
		public void IncorrectAccessThrows_Number()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					JsonValue json = false;
					var test = json.Number;
				});
		}
		[Test]
		public void IncorrectAccessThrows_String()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					JsonValue json = false;
					var test = json.String;
				});
		}
		[Test]
		public void IncorrectAccessThrows_Boolean()
		{
			Assert.Throws<JsonValueIncorrectTypeException>(() =>
				{
					JsonValue json = 4;
					var test = json.Boolean;
				});
		}
		[Test]
		public void IncorrectAccessIgnores_Object()
		{
			JsonOptions.ThrowOnIncorrectTypeAccess = false;

			try
			{
				JsonValue json = false;
				var test = json.Object;

				Assert.IsNull(test);
			}
			finally
			{
				JsonOptions.ThrowOnIncorrectTypeAccess = true;
			}
		}
		[Test]
		public void IncorrectAccessIgnores_Array()
		{
			JsonOptions.ThrowOnIncorrectTypeAccess = false;

			try
			{
				JsonValue json = false;
				var test = json.Array;

				Assert.IsNull(test);
			}
			finally
			{
				JsonOptions.ThrowOnIncorrectTypeAccess = true;
			}
		}
		[Test]
		public void IncorrectAccessIgnores_Number()
		{
			JsonOptions.ThrowOnIncorrectTypeAccess = false;

			try
			{
				JsonValue json = false;
				var test = json.Number;

				Assert.AreEqual(default(double), test);

			}
			finally
			{
				JsonOptions.ThrowOnIncorrectTypeAccess = true;
			}
		}
		[Test]
		public void IncorrectAccessIgnores_String()
		{
			JsonOptions.ThrowOnIncorrectTypeAccess = false;

			try
			{
				JsonValue json = false;
				var test = json.String;

				Assert.IsNull(test);
			}
			finally
			{
				JsonOptions.ThrowOnIncorrectTypeAccess = true;
			}
		}
		[Test]
		public void IncorrectAccessIgnores_Boolean()
		{
			JsonOptions.ThrowOnIncorrectTypeAccess = false;

			try
			{
				JsonValue json = false;
				var test = json.Boolean;

				Assert.AreEqual(default(bool), test);

			}
			finally
			{
				JsonOptions.ThrowOnIncorrectTypeAccess = true;
			}
		}

		#endregion

		#region DefaultArrayEquality

		[Test]
		public void SequenceEqual()
		{
			JsonValue array1 = new JsonArray {1, false};
			JsonValue array2 = new JsonArray {1, false};

			Assert.IsTrue(array1.Equals(array2));
		}

		[Test]
		public void SequenceEqualOverride()
		{
			JsonValue array1 = new JsonArray {1, false};
			array1.Array.EqualityStandard = ArrayEquality.ContentsEqual;
			JsonValue array2 = new JsonArray {false, 1};

			Assert.IsTrue(array1.Equals(array2));
		}

		[Test]
		public void SequenceNotEqual()
		{
			JsonValue array1 = new JsonArray {1, false};
			JsonValue array2 = new JsonArray {false, 1};

			Assert.IsFalse(array1.Equals(array2));
		}

		[Test]
		public void ContentsEqual()
		{
			JsonValue array1 = new JsonArray {1, false};
			JsonValue array2 = new JsonArray {1, false};

			Assert.IsTrue(array1.Equals(array2));
		}

		[Test]
		public void ContentsNotEqual()
		{
			JsonOptions.DefaultArrayEquality = ArrayEquality.ContentsEqual;

			JsonValue array1 = new JsonArray {1, false};
			JsonValue array2 = new JsonArray {false, 1};

			try
			{
				Assert.IsTrue(array1.Equals(array2));

			}
			finally
			{
				JsonOptions.DefaultArrayEquality = ArrayEquality.SequenceEqual;
			}
		}

		[Test]
		public void ContentsNotEqualOverride()
		{
			JsonOptions.DefaultArrayEquality = ArrayEquality.ContentsEqual;

			JsonValue array1 = new JsonArray {1, false};
			array1.Array.EqualityStandard = ArrayEquality.SequenceEqual;
			JsonValue array2 = new JsonArray {false, 1};

			try
			{
				Assert.IsFalse(array1.Equals(array2));

			}
			finally
			{
				JsonOptions.DefaultArrayEquality = ArrayEquality.SequenceEqual;
			}
		}

		#endregion
	}
}
