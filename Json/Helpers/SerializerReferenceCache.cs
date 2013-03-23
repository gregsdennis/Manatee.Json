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
 
	File Name:		SerializerReferenceCache.cs
	Namespace:		Manatee.Json.Helpers
	Class Name:		SerializerReferenceCache
	Purpose:		Maintains a cache of references used by the serializer to
					catch circular references.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Enumerations;

namespace Manatee.Json.Helpers
{
	internal class SerializerReferenceCache : List<ISerializerReferenceRecord>, ISerializerReferenceCache
	{
		private const string DefKey = "#Define";

		public ISerializerReferenceRecord FindRecord(object obj)
		{
			if (obj is string) return null;
			return this.FirstOrDefault(record => ReferenceEquals(record.Object, obj));
		}
		public void AddReference(string guid, object owner, PropertyInfo prop)
		{
			var g = Guid.Parse(guid);
			var record = this.FirstOrDefault(r => r.ReferenceId.Equals(g));
			if (record == null) return;
			record.References.Add(new PropertyReference {Owner = owner, Info = prop});
		}
		public void ReconcileJsonReferences()
		{
			var records = this.Where(record => record.IsReferenced);
			foreach (var record in records)
			{
				switch (record.Json.Type)
				{
					case JsonValueType.Object:
						record.Json.Object.Add(DefKey, record.ReferenceId.ToString());
						break;
					case JsonValueType.Array:
						record.Json.Array.Add(new JsonObject{{DefKey, record.ReferenceId.ToString()}});
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
		public void ReconcileObjectReferences()
		{
			foreach (var record in this)
			{
				foreach (var prop in record.References)
				{
					prop.Info.SetValue(prop.Owner, record.Object, null);
				}
			}
		}
	}
}
