using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal static class JsonPathExpressionParser
	{
		private static readonly List<IJsonPathExpressionParser> Parsers;

		static JsonPathExpressionParser()
		{
			Parsers = typeof(JsonPathExpressionParser).GetTypeInfo().Assembly.DefinedTypes
													  .Where(t => typeof(IJsonPathExpressionParser).GetTypeInfo().IsAssignableFrom(t) && t.IsClass)
													  .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                          .Cast<IJsonPathExpressionParser>()
			                                          .ToList();
		}

		public static string Parse<T, TIn>(string source, ref int index, out Expression<T, TIn> expr)
		{
			var error = Parse(source, ref index, out ExpressionTreeNode<TIn> root);
			if (error != null)
			{
				expr = null;
				return error;
			}
			if (root == null)
			{
				expr = null;
				return "No expression found.";
			}

			expr = new Expression<T, TIn>(root);
			return null;
		}
		public static string Parse<TIn>(string source, ref int index, out ExpressionTreeNode<TIn> root)
		{
			var nodes = new List<ExpressionTreeNode<TIn>>();
			var length = source.Length;
			ExpressionTreeNode<TIn> node;
			root = null;
			do
			{
				var errorMessage = source.SkipWhiteSpace(ref index, length, out char c);
				if (errorMessage != null) return errorMessage;

				IJsonPathExpressionParser foundParser = null;
				foreach (var parser in Parsers)
				{
					if (parser.Handles(source, index))
					{
						foundParser = parser;
						break;
					}
				}

				if (foundParser == null) return "Unrecognized JSON Path Expression element.";
				errorMessage = foundParser.TryParse(source, ref index, out node);
				if (errorMessage != null) return errorMessage;
				if (node != null)
					nodes.Add(node);
			} while (index < length && node != null);

			root = nodes.Count == 1
				       ? _CheckNode(nodes[0], null)
				       : _BuildTree(nodes);
			return null;
		}

		private static ExpressionTreeNode<T> _BuildTree<T>(List<ExpressionTreeNode<T>> nodes)
		{
			if (!nodes.Any()) return null;
			var minPriority = nodes.Min(n => n.Priority);
			var root = nodes.Last(n => n.Priority == minPriority);
			if (root is ExpressionTreeBranch<T> branch && branch.Right == null && branch.Left == null)
			{
				var split = nodes.LastIndexOf(root);
				var left = nodes.Take(split).ToList();
				var right = nodes.Skip(split + 1).ToList();
				branch.Left = _CheckNode(_BuildTree(left), branch);
				branch.Right = _CheckNode(_BuildTree(right), branch);
			}
			if (root is NotExpression<T> not)
			{
				var split = nodes.IndexOf(root);
				var right = nodes.Skip(split + 1).FirstOrDefault();
				not.Root = _CheckNode(right, null);
			}
			return root;
		}

		private static ExpressionTreeNode<T> _CheckNode<T>(ExpressionTreeNode<T> node, ExpressionTreeBranch<T> root)
		{
			if (node is NameExpression<T> named && (root == null || root.Priority == 0))
			{
				return new HasPropertyExpression<T>
				{
					Path = named.Path,
					IsLocal = named.IsLocal,
					Name = named.Name
				};
			}
			return node;
		}
	}
}