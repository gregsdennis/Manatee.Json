using System;
using Manatee.Json.Serialization.Internal;
using Manatee.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Tests.Json.Serialization.Internal
{
	[TestClass]
	public class TypeGeneratorTest
	{
		[TestMethod]
		public void TypeCreation_Success()
		{
			var instance = TypeGenerator.Default.Generate<IInterface>();

			Assert.IsNotNull(instance);
		}
		[TestMethod]
		public void PropertyReadAndWrite_Success()
		{
			string stringProp = "test";

			var instance = TypeGenerator.Default.Generate<IInterface>();
			instance.RequiredProp = stringProp;

			Assert.AreEqual(stringProp, instance.RequiredProp);
		}
		[TestMethod]
		public void MethodCall_Success()
		{
			var instance = TypeGenerator.Default.Generate<IInterface>();

			var actual = instance.RequiredMethod<int, IConvertible>(2.0);

			Assert.AreEqual(default(int), actual);
		}
		[TestMethod]
		public void EventSubscription_Success()
		{
			var instance = TypeGenerator.Default.Generate<IInterface>();

			EventHandler handler = (o, e) => { };

			instance.RequiredEvent += handler;
			instance.RequiredEvent -= handler;
		}
		[TestMethod]
		public void CacheTypes_Success()
		{
			var instance = TypeGenerator.Default.Generate<IInterface>();
			var instance2 = TypeGenerator.Default.Generate<IInterface>();

			Assert.AreNotSame(instance, instance2);
			Assert.AreEqual(instance.GetType(), instance2.GetType());
			Console.WriteLine("Caching succeeded.");
		}
	}
}
