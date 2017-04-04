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
			Parsers = typeof(JsonPathExpressionParser).TypeInfo().Assembly.GetTypes()
													  .Where(t => typeof(IJsonPathExpressionParser).IsAssignableFrom(t) && t.TypeInfo().IsClass)
													  .Select(Activator.CreateInstance)
			                                          .Cast<IJsonPathExpressionParser>()
			                                          .ToList();
		}

		public static string Parse<T, TIn>(string source, ref int index, out Expression<T, TIn> expr)
		{
			ExpressionTreeNode<TIn> root;
			var error = Parse(source, ref index, out root);
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
				char c;
				var errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return errorMessage;
				var substring = source.Substring(index);
				var parser = Parsers.FirstOrDefault(p => p.Handles(substring));
				if (parser == null) return "Unrecognized JSON Path Expression element.";
				errorMessage = parser.TryParse(source, ref index, out node);
				if (errorMessage != null) return errorMessage;
				if (node != null)
					nodes.Add(node);
			} while (index < length && node != null);

			root = nodes.Count == 1
				       ? CheckNode(nodes[0], null)
				       : BuildTree(nodes);
			return null;
		}

		private static ExpressionTreeNode<T> BuildTree<T>(List<ExpressionTreeNode<T>> nodes)
		{
			if (!nodes.Any()) return null;
			var minPriority = nodes.Min(n => n.Priority);
			var root = nodes.Last(n => n.Priority == minPriority);
			var branch = root as ExpressionTreeBranch<T>;
			if (branch != null && branch.Right == null && branch.Left == null)
			{
				var split = nodes.LastIndexOf(root);
				var left = nodes.Take(split).ToList();
				var right = nodes.Skip(split + 1).ToList();
				branch.Left = CheckNode(BuildTree(left), branch);
				branch.Right = CheckNode(BuildTree(right), branch);
			}
			var not = root as NotExpression<T>;
			if (not != null)
			{
				var split = nodes.IndexOf(root);
				var right = nodes.Skip(split + 1).FirstOrDefault();
				not.Root = CheckNode(right, null);
			}
			return root;
		}

		private static ExpressionTreeNode<T> CheckNode<T>(ExpressionTreeNode<T> node, ExpressionTreeBranch<T> root)
		{
			var named = node as NameExpression<T>;
			if (named != null && (root == null || root.Priority == 0))
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