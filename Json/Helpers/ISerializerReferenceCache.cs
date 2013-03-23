using System.Collections.Generic;
using System.Reflection;

namespace Manatee.Json.Helpers
{
	internal interface ISerializerReferenceCache : IList<ISerializerReferenceRecord>
	{
		ISerializerReferenceRecord FindRecord(object obj);
		void AddReference(string guid, object owner, PropertyInfo prop);
		void ReconcileJsonReferences();
		void ReconcileObjectReferences();
	}
}