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
 
	File Name:		XmlNamespacePair.cs
	Namespace:		Manatee.Json.Internal
	Class Name:		XmlNamespacePair
	Purpose:		Maintains a cache of namespace pairs defined by a given
					XML element.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Manatee.Json.Internal
{
	internal class XmlNamespaceRegistry
	{
		[ThreadStatic]
		private static XmlNamespaceRegistry _instance;

		public static XmlNamespaceRegistry Instance { get { return _instance ?? (_instance = new XmlNamespaceRegistry()); } }

		private XmlNamespaceRegistry() {}

		private readonly Dictionary<XElement, List<XmlNamespacePair>> _registry = new Dictionary<XElement, List<XmlNamespacePair>>();
		private readonly Dictionary<string, Stack<string>> _stack = new Dictionary<string, Stack<string>>();

		public void RegisterElement(XElement element)
		{
			if (_registry.ContainsKey(element))
				_registry.Remove(element);
			var attributes = element.Attributes().Where(a => a.IsNamespaceDeclaration);
			_registry.Add(element, new List<XmlNamespacePair>(attributes.Select(a => new XmlNamespacePair
			                                                                         	{
			                                                                         		Namespace = a.Value,
			                                                                         		Label = a.Name.LocalName
			                                                                         	})));
		}
		public void UnRegisterElement(XElement element)
		{
			_registry.Remove(element);
		}
		public bool ElementDefinesNamespace(XElement element, string space)
		{
			return _registry.ContainsKey(element) && _registry[element].Any(pair => pair.Namespace == space);
		}
		public string GetLabel(XElement element, string space)
		{
			return _registry[element].First(pair => pair.Namespace == space).Label;
		}

		public void Register(string label, string space)
		{
			if (!_stack.ContainsKey(label))
				_stack.Add(label, new Stack<string>());
			_stack[label].Push(space);
		}
		public void Unregister(string label)
		{
			_stack[label].Pop();
			if (_stack[label].Count == 0)
				_stack.Remove(label);
		}
		public string GetNamespace(string label)
		{
			if (!_stack.ContainsKey(label)) return null;
			return _stack[label].Count == 0 ? null : _stack[label].Peek();
		}
	}
}
