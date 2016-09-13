using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions.Parsing
{
	internal static class JsonPathExpressionParser
	{
		private static readonly List<IJsonPathExpressionParser> Parsers;

		static JsonPathExpressionParser()
		{
			Parsers = typeof(JsonPathExpressionParser).Assembly.GetTypes()
			                                          .Where(t => typeof(IJsonPathExpressionParser).IsAssignableFrom(t) && t.IsClass)
			                                          .Select(Activator.CreateInstance)
			                                          .Cast<IJsonPathExpressionParser>()
			                                          .ToList();
		}

		public static string Parse<T, TIn>(string source, ref int index, out Expression<T, TIn> expr)
		{
			var nodes = new List<ExpressionTreeNode<TIn>>();
			var length = source.Length;
			expr = null;
			while (index < length)
			{
				char c;
				var errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return errorMessage;
				var i = index;
				var parser = Parsers.FirstOrDefault(p => p.Handles(source.Substring(i)));
				if (parser == null) return "Unrecognized JSON Path Expression element.";
				ExpressionTreeNode<TIn> node;
				errorMessage = parser.TryParse(source, ref index, out node);
				if (errorMessage != null) return errorMessage;
				nodes.Add(node);
			}

			expr = new Expression<T, TIn>(BuildTree(nodes));
			return null;
		}

		private static ExpressionTreeNode<T> BuildTree<T>(IList<ExpressionTreeNode<T>> nodes)
		{
			if (!nodes.Any()) return null;
			var minPriority = nodes.Min(n => n.Priority);
			var root = nodes.Last(n => n.Priority == minPriority);
			var branch = root as ExpressionTreeBranch<T>;
			if (branch != null)
			{
				var split = nodes.IndexOf(root);
				var left = nodes.Take(split).ToList();
				var right = nodes.Skip(split + 1).ToList();
				branch.Left = BuildTree(left);
				branch.Right = BuildTree(right);
			}
			var not = root as NotExpression<T>;
			if (not != null)
			{
				var split = nodes.IndexOf(root);
				var right = nodes.Skip(split + 1).FirstOrDefault();
				not.Root = right;
			}
			return root;
		}
	}
}