using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Manatee.Json.Enumerations;

namespace Manatee.Json.Helpers
{
	class SerializerReferenceCache : List<SerializerReferenceRecord>
	{
		private const string DefKey = "#Define";

		public SerializerReferenceRecord FindRecord(object obj)
		{
			if (obj is string) return null;
			return this.Where(record => ReferenceEquals(record.Object, obj)).FirstOrDefault();
		}
		public void AddReference(string guid, object owner, PropertyInfo prop)
		{
			var g = Guid.Parse(guid);
			var record = this.Where(r => r.ReferenceID.Equals(g)).FirstOrDefault();
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
						record.Json.Object.Add(DefKey, record.ReferenceID.ToString());
						break;
					case JsonValueType.Array:
						record.Json.Array.Add(new JsonObject{{DefKey, record.ReferenceID.ToString()}});
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
