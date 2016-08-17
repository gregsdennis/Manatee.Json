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
 
	File Name:		ISerializationDelegateProvider.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		ISerializationDelegateProvider
	Purpose:		Defines methods required to provide registration methods
					to the JsonSerializerTypeRegistry class.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization.Internal
{
	internal interface ISerializationDelegateProvider
	{
		bool CanHandle(Type type);
		JsonSerializationTypeRegistry.ToJsonDelegate<T> GetEncoder<T>();
		JsonSerializationTypeRegistry.FromJsonDelegate<T> GetDecoder<T>();
	}
}