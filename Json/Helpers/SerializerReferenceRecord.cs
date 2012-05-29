using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Manatee.Json.Helpers
{
	class SerializerReferenceRecord
	{
		private Guid _guid;

		public object Object { get; set; }
		public Guid ReferenceID { get { return _guid; } }
		public JsonValue Json { get; set; }
		public bool IsReferenced { get; set; }
		public List<PropertyReference> References { get; private set; }

		public SerializerReferenceRecord()
		{
			_guid = Guid.NewGuid();
			References = new List<PropertyReference>();
		}
		public SerializerReferenceRecord(string guid)
		{
			_guid = Guid.Parse(guid);
			References = new List<PropertyReference>();
		}
	}
}
