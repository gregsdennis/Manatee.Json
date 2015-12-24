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
 
	File Name:		SerializationPair.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		SerializationPair
	Purpose:		Defines an equivalence between an object and its representative
					JSON.

***************************************************************************************/

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