using System;
using Manatee.Json.Serialization.Internal;
using Manatee.Json.Tests.Test_References;
using NUnit.Framework;

namespace Manatee.Json.Tests.Serialization.Internal
{
	[TestFixture]
	public class TypeGeneratorTest
	{
		[Test]
		public void TypeCreation_Success()
		{
			var instance = TypeGenerator.Generate(typeof(IInterface));

			Assert.IsNotNull(instance);
		}
		[Test]
		public void PropertyReadAndWrite_Success()
		{
			var stringProp = "test";

			var instance = (IInterface) TypeGenerator.Generate(typeof(IInterface));
			instance.RequiredProp = stringProp;

			Assert.AreEqual(stringProp, instance.RequiredProp);
		}
		[Test]
		public void MethodCall_Success()
		{
			var instance = (IInterface) TypeGenerator.Generate(typeof(IInterface));

			var actual = instance.RequiredMethod<int, IConvertible>(2.0);

			Assert.AreEqual(default(int), actual);
		}
		[Test]
		public void EventSubscription_Success()
		{
			var instance = (IInterface) TypeGenerator.Generate(typeof(IInterface));

			EventHandler handler = (o, e) => { };

			instance.RequiredEvent += handler;
			instance.RequiredEvent -= handler;
		}
		[Test]
		public void CacheTypes_Success()
		{
			var instance = TypeGenerator.Generate(typeof(IInterface));
			var instance2 = TypeGenerator.Generate(typeof(IInterface));

			Assert.AreNotSame(instance, instance2);
			Assert.AreEqual(instance.GetType(), instance2.GetType());
			Console.WriteLine("Caching succeeded.");
		}
	}
}
