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
 
	File Name:		Draft04SchemaTest.cs
	Namespace:		Manatee.Json.Tests.Schema
	Class Name:		Draft04SchemaTest
	Purpose:		Tests that the Draft04 schema object validates itself.

***************************************************************************************/

using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests.Schema
{
	[TestClass]
	public class Draft04SchemaTest
	{
		 [TestMethod]
		 public void Draft04IsSelfValidating()
		 {
			 var json = JsonSchema.Draft04.ToJson(null);
			 var results = JsonSchema.Draft04.Validate(json);

			 Assert.IsTrue(results.Valid);
			 Assert.AreEqual(0, results.Errors.Count());
		 }
	}
}