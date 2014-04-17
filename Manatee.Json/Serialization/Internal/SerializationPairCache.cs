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
 
	File Name:		SerializationPairCache.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		SerializationPairCache
	Purpose:		Manages pairs of objects and their respective JSON.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPairCache : Dictionary<Guid, SerializationPair>
	{
		public bool Contains(object obj)
		{
			return Values.Any(v => ReferenceEquals(v.Object, obj));
		}
		public Guid GetKey(object obj)
		{
			return this.First(v => ReferenceEquals(v.Value.Object, obj)).Key;
		}
	}
}