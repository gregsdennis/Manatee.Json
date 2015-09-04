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
 
	File Name:		JsonSerializableImplementationClass.cs
	Namespace:		Manatee.Tests.Test_References
	Class Name:		JsonSerializableImplementationClass
	Purpose:		Basic class that implements IJsonSerializable to be used in
					testing the Manatee.Json library.

***************************************************************************************/

using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Test_References
{
	public class JsonSerializableImplementationClass : ImplementationClass, IJsonSerializable
	{
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			RequiredProp = json.Object["requiredProp"].String;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return new JsonObject { { "requiredProp", RequiredProp } };
		}
	}
}