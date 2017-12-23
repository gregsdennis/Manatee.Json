using Manatee.Json.Path;
using NUnit.Framework;

namespace Manatee.Json.Tests.Path
{
	[TestFixture]
	public class ToStringTest
	{
		private static void Run(string expected, JsonPath path)
		{
			var actual = path.ToString();
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void JustRoot()
		{
			Run("$", new JsonPath());
		}

		#region Dot Operator

		[Test]
		public void ObjectKey()
		{
			Run("$.name", JsonPathWith.Name("name"));
		}
		[Test]
		public void ObjectChain()
		{
			Run("$.name.test", JsonPathWith.Name("name").Name("test"));
		}
		[Test]
		public void Length()
		{
			Run("$.length", JsonPathWith.Length());
		}
		[Test]
		public void EmptyKey()
		{
			Run("$.''", JsonPathWith.Name(""));
		}
		[Test]
		public void EmptySearch()
		{
			Run("$..''", JsonPathWith.Search(""));
		}

		#endregion

		#region Indexed Arrays

		[Test]
		public void ArrayIndex_SingleConstant()
		{
			Run("$[0]", JsonPathWith.Array(0));
		}
		[Test]
		public void ArrayIndex_SingleSlice()
		{
			Run("$[0:6]", JsonPathWith.Array(new Slice(0, 6)));
		}
		[Test]
		public void ArrayIndex_SingleSliceStartEndStep()
		{
			Run("$[0:6:2]", JsonPathWith.Array(new Slice(0, 6, 2)));
		}
		[Test]
		public void ArrayIndex_SingleSliceStart()
		{
			Run("$[0:]", JsonPathWith.Array(new Slice(0, null)));
		}
		[Test]
		public void ArrayIndex_SingleSliceEnd()
		{
			Run("$[:6]", JsonPathWith.Array(new Slice(null, 6)));
		}
		[Test]
		public void ArrayIndex_SingleSliceStartStep()
		{
			Run("$[0::2]", JsonPathWith.Array(new Slice(0, null, 2)));
		}
		[Test]
		public void ArrayIndex_SingleEndStep()
		{
			Run("$[:6:2]", JsonPathWith.Array(new Slice(null, 6, 2)));
		}
		[Test]
		public void ArrayIndex_SingleSliceStep()
		{
			Run("$[::2]", JsonPathWith.Array(new Slice(null, null, 2)));
		}
		[Test]
		public void ArrayIndex_ConstantSlice()
		{
			Run("$[1,4:6]", JsonPathWith.Array(1, new Slice(4, 6)));
		}

		#endregion

		#region Index Expression Arrays

		[Test]
		public void ArrayIndexExpression_LocalLength()
		{
			Run("$[(@.length)]", JsonPathWith.Array(jv => jv.Length()));
		}
		[Test]
		public void ArrayIndexExpression_RootLength()
		{
			Run("$[($.length)]", JsonPathWith.Array(jv => JsonPathRoot.Length()));
		}
		[Test]
		public void ArrayIndexExpression_NameLength()
		{
			Run("$[(@.name.length)]", JsonPathWith.Array(jv => jv.Name("name").Length()));
		}
		[Test]
		public void ArrayIndexExpression_Addition()
		{
			Run("$[(@.length+1)]", JsonPathWith.Array(jv => jv.Length() + 1));
		}
		[Test]
		public void ArrayIndexExpression_Subtraction()
		{
			Run("$[(@.length-1)]", JsonPathWith.Array(jv => jv.Length() - 1));
		}
		[Test]
		public void ArrayIndexExpression_Multiplication()
		{
			Run("$[(@.length*1)]", JsonPathWith.Array(jv => jv.Length() * 1));
		}
		[Test]
		public void ArrayIndexExpression_Division()
		{
			Run("$[(@.length/1)]", JsonPathWith.Array(jv => jv.Length() / 1));
		}
		[Test]
		public void ArrayIndexExpression_Modulus()
		{
			Run("$[(@.length%1)]", JsonPathWith.Array(jv => jv.Length() % 1));
		}

		#endregion

		#region Filter Expression Arrays

		[Test]
		public void ArrayFilterExpression_LocalNameExists()
		{
			Run("$[?(@.test)]", JsonPathWith.Array(jv => jv.HasProperty("test")));
		}
		[Test]
		public void ArrayFilterExpression_LocalLengthEqualsInt()
		{
			Run("$[?(@.length == 1)]", JsonPathWith.Array(jv => jv.Length() == 1));
		}
		[Test]
		public void ArrayFilterExpression_LocalNameEqualsString()
		{
			Run("$[?(@.test == \"string\")]", JsonPathWith.Array(jv => jv.Name("test") == "string"));
		}
		[Test]
		public void ArrayFilterExpression_LocalNameChainEqualsBoolean()
		{
			Run("$[?(@.name.test == false)]", JsonPathWith.Array(jv => jv.Name("name").Name("test") == false));
		}
		[Test]
		public void ArrayFilterExpression_RootLengthNotEqualsValue()
		{
			Run("$[?($.length != 1)]", JsonPathWith.Array(jv => JsonPathRoot.Length() != 1));
		}
		[Test]
		public void ArrayFilterExpression_NameLengthLessThanValue()
		{
			Run("$[?(@.name.length < 1)]", JsonPathWith.Array(jv => jv.Name("name").Length() < 1));
		}
		[Test]
		public void ArrayFilterExpression_AdditionLessThanEqualValue()
		{
			Run("$[?(@.length <= 1)]", JsonPathWith.Array(jv => jv.Length() <= 1));
		}
		[Test]
		public void ArrayFilterExpression_SubtractionGreaterThanValue()
		{
			Run("$[?((@.length-1) > 2)]", JsonPathWith.Array(jv => jv.Length() - 1 > 2));
		}
		[Test]
		public void ArrayFilterExpression_MultiplicationGreaterThanEqualValue()
		{
			Run("$[?((@.length*1) >= 2)]", JsonPathWith.Array(jv => jv.Length() * 1 >= 2));
		}
		[Test]
		public void ArrayFilterExpression_DivisionEqualValue()
		{
			Run("$[?((@.length/1) == 2)]", JsonPathWith.Array(jv => jv.Length() / 1 == 2));
		}
		[Test]
		public void ArrayFilterExpression_ModulusEqualValue()
		{
			Run("$[?((@.length%1) == 2)]", JsonPathWith.Array(jv => jv.Length() % 1 == 2));
		}
		[Test]
		public void ArrayIndexExpression_IndexOfNumber()
		{
			Run("$[?(@.indexOf(5) == 4)]", JsonPathWith.Array(jv => jv.IndexOf(5) == 4));
		}
		[Test]
		public void ArrayIndexExpression_IndexOfBoolean()
		{
			Run("$[?(@.indexOf(false) == 4)]", JsonPathWith.Array(jv => jv.IndexOf(false) == 4));
		}
		[Test]
		public void ArrayIndexExpression_IndexOfString()
		{
			Run("$[?(@.indexOf(\"string\") == 4)]", JsonPathWith.Array(jv => jv.IndexOf("string") == 4));
		}
		[Test]
		public void ArrayIndexExpression_IndexOfArray()
		{
			var arr = new JsonArray {1, 2, 3};
			Run("$[?(@.indexOf([1,2,3]) == 4)]", JsonPathWith.Array(jv => jv.IndexOf(arr) == 4));
		}
		[Test]
		public void ArrayIndexExpression_IndexOfObject()
		{
			var obj = new JsonObject {{"key", "value"}};
			Run("$[?(@.indexOf({\"key\":\"value\"}) == 4)]", JsonPathWith.Array(jv => jv.IndexOf(obj) == 4));
		}

		#endregion
	}
}
