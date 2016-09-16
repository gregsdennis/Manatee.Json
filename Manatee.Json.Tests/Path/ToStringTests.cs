using Manatee.Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Path
{
	[TestClass]
	public class ToStringTests
	{
		private void Run(string expected, JsonPath path)
		{
			var actual = path.ToString();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void JustRoot()
		{
			Run("$", new JsonPath());
		}
		[TestMethod]
		public void ObjectKey()
		{
			Run("$.name", JsonPathWith.Name("name"));
		}
		[TestMethod]
		public void ArrayIndex_SingleConstant()
		{
			Run("$[0]", JsonPathWith.Array(0));
		}
		[TestMethod]
		public void ArrayIndex_SingleSlice()
		{
			Run("$[0:6]", JsonPathWith.Array(new Slice(0, 6)));
		}
		[TestMethod]
		public void ArrayIndex_SingleSliceStartEndStep()
		{
			Run("$[0:6:2]", JsonPathWith.Array(new Slice(0, 6, 2)));
		}
		[TestMethod]
		public void ArrayIndex_SingleSliceStart()
		{
			Run("$[0:]", JsonPathWith.Array(new Slice(0, null)));
		}
		[TestMethod]
		public void ArrayIndex_SingleSliceEnd()
		{
			Run("$[:6]", JsonPathWith.Array(new Slice(null, 6)));
		}
		[TestMethod]
		public void ArrayIndex_SingleSliceStartStep()
		{
			Run("$[0::2]", JsonPathWith.Array(new Slice(0, null, 2)));
		}
		[TestMethod]
		public void ArrayIndex_SingleEndStep()
		{
			Run("$[:6:2]", JsonPathWith.Array(new Slice(null, 6, 2)));
		}
		[TestMethod]
		public void ArrayIndex_SingleSliceStep()
		{
			Run("$[::2]", JsonPathWith.Array(new Slice(null, null, 2)));
		}
		[TestMethod]
		public void ArrayIndex_ConstantSlice()
		{
			Run("$[1,4:6]", JsonPathWith.Array(1, new Slice(4, 6)));
		}
	}
}
