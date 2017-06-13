﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Manatee.Json.Internal
{
	internal class XmlNamespaceRegistry
	{
		[ThreadStatic]
		private static XmlNamespaceRegistry _instance;

		public static XmlNamespaceRegistry Instance => _instance ?? (_instance = new XmlNamespaceRegistry());

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
			List<XmlNamespacePair> entry;
			return _registry.TryGetValue(element, out entry) && entry.Any(pair => pair.Namespace == space);
		}
		public string GetLabel(XElement element, string space)
		{
			return _registry[element].First(pair => pair.Namespace == space).Label;
		}

		public void Register(string label, string space)
		{
			Stack<string> entry;
			if (!_stack.TryGetValue(label, out entry))
			{
				entry = new Stack<string>();
				_stack.Add(label, entry);
			}
			entry.Push(space);
		}
		public void Unregister(string label)
		{
			_stack[label].Pop();
			if (_stack[label].Count == 0)
				_stack.Remove(label);
		}
		public string GetNamespace(string label)
		{
			Stack<string> entry;
			if (!_stack.TryGetValue(label, out entry)) return null;
			return _stack.TryGetValue(label, out entry) && entry.Count != 0 ? entry.Peek() : null;
		}
	}
}
