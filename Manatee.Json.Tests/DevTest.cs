/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		DevTest.cs
	Namespace:		Manatee.Tests
	Class Name:		DevTest
	Purpose:		Provides a single method through which one can test any specific
					scenario.  Should be marked [Ignore] when committing to the
					repository.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manatee.Json.Tests
{
	[TestClass]
	public class DevTest
	{
		[TestMethod]
		//[Ignore]
		public void Test1()
		{
			var geoSchema = new ObjectSchema
			{
				Properties = new JsonSchemaPropertyDefinitionCollection
						{
							new JsonSchemaPropertyDefinition
								{
									Name = "latitude",
									Type = new NumberSchema(),
									IsRequired = true
								},
							new JsonSchemaPropertyDefinition
								{
									Name = "longitude",
									Type = new NumberSchema(),
									IsRequired = false
								},
							new JsonSchemaPropertyDefinition()
								{
									Name = "stringData",
									Type = new StringSchema
										{
											Format = StringFormat.HostName
										},
									IsRequired = true
								}
						}
			};
			var geoJson = new JsonObject
				{
					{"latitude", 95.4},
					{"longitude", 36.8},
					{"stringData", ""}
				};
			var result = geoSchema.Validate(geoJson);
			Console.WriteLine("geoJson valid? {0}", result.Valid);
			foreach (var error in result.Errors)
			{
				Console.WriteLine("    {0}", error);
			}
			if (result.Errors.Any())
				throw new Exception();
		}
		[TestMethod]
		//[Ignore]
		public void Test2()
		{
			// Having some problems with generating schema from complex or immutable types.
			// For example, the system can't generate for KeyValuePair<,> since the properties aren't read/write.
			// Try Dictionary<string, int> or JsonObject (Dictionary<string, JsonValue>).
			var schema = JsonSchemaFactory.FromTypeBeta(typeof (Dictionary<string, int>));
			Console.WriteLine(schema.ToJson(null));
		}
		[TestMethod]
		public void QuotesTest()
		{
			var str = "[{\"id\":\"533f018805de2474035484e1\",\"closed\":false,\"dateLastActivity\":\"2014-04-16T19:28:54.092Z\",\"desc\":\"User Story #2072\",\"idBoard\":\"51b9e93ac6c1fb974b0002c8\",\"idList\":\"5342afd77c14f8c318f0ad53\",\"idShort\":2083,\"idAttachmentCover\":null,\"manualCoverAttachment\":false,\"name\":\"Change \\\"Remove Additional Deposits\\\" to \\\"Remove Pending Deposits\\\"\",\"pos\":1769472,\"due\":null,\"url\":\"https://trello.com/c/kZEIQxtL/2083-change-remove-additional-deposits-to-remove-pending-deposits\",\"subscribed\":true},{\"id\":\"533f018b14ad777818e5e49c\",\"closed\":false,\"dateLastActivity\":\"2014-04-16T19:28:54.114Z\",\"desc\":\"User Story #2072\",\"idBoard\":\"51b9e93ac6c1fb974b0002c8\",\"idList\":\"5342afd77c14f8c318f0ad53\",\"idShort\":2089,\"idAttachmentCover\":null,\"manualCoverAttachment\":false,\"name\":\"Domain changes\",\"pos\":1867776,\"due\":null,\"url\":\"https://trello.com/c/bEq7rzRa/2089-domain-changes\",\"subscribed\":true}]";
			var json = JsonValue.Parse(str);
			Console.WriteLine(json.GetIndentedString());
		}
	}
}