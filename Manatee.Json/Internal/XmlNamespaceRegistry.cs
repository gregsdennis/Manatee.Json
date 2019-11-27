using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Manatee.Json.Internal
{
	internal class XmlNamespaceRegistry
	{
		private class XmlNamespacePair
		{
			public string Namespace { get; }
			public string Label { get; }

			public XmlNamespacePair(string @namespace, string label)
			{
				Namespace = @namespace;
				Label = label;
			}
		}

		[ThreadStatic]
		private static XmlNamespaceRegistry? _instance;

		public static XmlNamespaceRegistry Instance => _instance ??= new XmlNamespaceRegistry();

		private XmlNamespaceRegistry() {}

		private readonly Dictionary<XElement, List<XmlNamespacePair>> _registry = new Dictionary<XElement, List<XmlNamespacePair>>();
		private readonly Dictionary<string, Stack<string>> _stack = new Dictionary<string, Stack<string>>();

		public void RegisterElement(XElement element)
		{
			if (_registry.ContainsKey(element))
				_registry.Remove(element);
			var attributes = element.Attributes().Where(a => a.IsNamespaceDeclaration);
			_registry.Add(element, new List<XmlNamespacePair>(attributes.Select(a => new XmlNamespacePair(a.Value, a.Name.LocalName))));
		}
		public void UnRegisterElement(XElement element)
		{
			_registry.Remove(element);
		}
		public bool ElementDefinesNamespace(XElement element, string space)
		{
			return _registry.TryGetValue(element, out List<XmlNamespacePair> entry) && entry.Any(pair => pair.Namespace == space);
		}
		public string GetLabel(XElement element, string space)
		{
			return _registry[element].First(pair => pair.Namespace == space).Label;
		}

		public void Register(string label, string space)
		{
			if (!_stack.TryGetValue(label, out Stack<string> entry))
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
		public string? GetNamespace(string label)
		{
			if (!_stack.TryGetValue(label, out Stack<string> entry)) return null;
			return _stack.TryGetValue(label, out entry) && entry.Count != 0 ? entry.Peek() : null;
		}
	}
}
