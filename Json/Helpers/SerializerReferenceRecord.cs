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
 
	File Name:		SerializerReferenceRecord.cs
	Namespace:		Manatee.Json.Helpers
	Class Name:		SerializerReferenceRecord
	Purpose:		A single reference record used by the serializer to catch
					circular references.

***************************************************************************************/
using System;
using System.Collections.Generic;

namespace Manatee.Json.Helpers
{
	internal interface ISerializerReferenceRecord
	{
		object Object { get; set; }
		Guid ReferenceId { get; }
		JsonValue Json { get; set; }
		bool IsReferenced { get; set; }
		List<PropertyReference> References { get; }
	}

	internal class SerializerReferenceRecord : ISerializerReferenceRecord
	{
		private readonly Guid _guid;

		public object Object { get; set; }
		public Guid ReferenceId { get { return _guid; } }
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
