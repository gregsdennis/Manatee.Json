/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		TypeGeneratorTest.cs
	Namespace:		Manatee.Json.Tests.Serialization.Internal
	Class Name:		TypeGeneratorTest
	Purpose:		This is a test class for TypeGenerator and is intended
					to contain all TypeGenerator Unit Tests

***************************************************************************************/
using System;
using Manatee.Json.Serialization.Internal;
using Manatee.Json.Tests.Test_References;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Serialization.Internal
{
	[TestClass]
	public class TypeGeneratorTest
	{
		[TestMethod]
		public void TypeCreation_Success()
		{
			var instance = TypeGenerator.Generate<IInterface>();

			Assert.IsNotNull(instance);
		}
		[TestMethod]
		public void PropertyReadAndWrite_Success()
		{
			string stringProp = "test";

			var instance = TypeGenerator.Generate<IInterface>();
			instance.RequiredProp = stringProp;

			Assert.AreEqual(stringProp, instance.RequiredProp);
		}
		[TestMethod]
		public void MethodCall_Success()
		{
			var instance = TypeGenerator.Generate<IInterface>();

			var actual = instance.RequiredMethod<int, IConvertible>(2.0);

			Assert.AreEqual(default(int), actual);
		}
		[TestMethod]
		public void EventSubscription_Success()
		{
			var instance = TypeGenerator.Generate<IInterface>();

			EventHandler handler = (o, e) => { };

			instance.RequiredEvent += handler;
			instance.RequiredEvent -= handler;
		}
		[TestMethod]
		public void CacheTypes_Success()
		{
			var instance = TypeGenerator.Generate<IInterface>();
			var instance2 = TypeGenerator.Generate<IInterface>();

			Assert.AreNotSame(instance, instance2);
			Assert.AreEqual(instance.GetType(), instance2.GetType());
			Console.WriteLine("Caching succeeded.");
		}
	}
}
