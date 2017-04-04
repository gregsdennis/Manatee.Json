using System.Collections.Generic;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPair
	{
		private readonly List<MemberReference> _references;

		public JsonValue Json { get; set; }
		public object Object { get; set; }
		public int UsageCount { get; set; }
		public bool DeserializationIsComplete { get; set; }

		public SerializationPair()
		{
			_references = new List<MemberReference>();
		}

		public void AddReference(PropertyInfo property, object obj)
		{
			_references.Add(new PropertyReference {Info = property, Owner = obj});
		}
		public void AddReference(FieldInfo field, object obj)
		{
			_references.Add(new FieldReference {Info = field, Owner = obj});
		}
		public void Reconcile()
		{
			foreach (var memberReference in _references)
			{
				memberReference.SetValue(memberReference.Owner, Object);
			}
			_references.Clear();
		}
		public override string ToString()
		{
			return $"Usage: {UsageCount}, Object: {Object}, Json: {Json}";
		}
	}
}