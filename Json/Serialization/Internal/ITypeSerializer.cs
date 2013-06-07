﻿/***************************************************************************************

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
 
	File Name:		ITypeSerializer.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		ITypeSerializer
	Purpose:		Defines methods to convert between types and JsonValues.

***************************************************************************************/

namespace Manatee.Json.Serialization.Internal
{
	internal interface ITypeSerializer
	{
		JsonValue SerializeType<T>(JsonSerializer serializer);
		void DeserializeType<T>(JsonValue json, JsonSerializer serializer);
	}
}