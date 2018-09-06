using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Serialization;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Schema
{
	public static class SchemaKeywordCatalog
	{
		private static readonly Dictionary<string, List<Type>> _cache = new Dictionary<string, List<Type>>();
		private static readonly ConstructorResolver _resolver = new ConstructorResolver();

		static SchemaKeywordCatalog()
		{
			var keywordTypes = typeof(IJsonSchemaKeyword).GetTypeInfo().Assembly.DefinedTypes
				.Where(t => typeof(IJsonSchemaKeyword).GetTypeInfo().IsAssignableFrom(t) &&
				            !t.IsAbstract)
				.Select(ti => ti.AsType());
			var method = typeof(SchemaKeywordCatalog).GetTypeInfo().DeclaredMethods
				.Single(m => m.Name == nameof(Add));
			foreach (var keywordType in keywordTypes)
			{
				method.MakeGenericMethod(keywordType).Invoke(null, new object[] { });
			}
		}

		public static void Add<T>()
			where T : IJsonSchemaKeyword, new()
		{
			var keyword = (T) _resolver.Resolve(typeof(T));
			if (!_cache.TryGetValue(keyword.Name, out var list))
			{
				list = new List<Type>();
				_cache[keyword.Name] = list;
			}
			if (!list.Contains(typeof(T)))
				list.Add(typeof(T));
		}

		internal static IJsonSchemaKeyword Build(string keywordName, JsonValue json, JsonSerializer serializer)
		{
			if (!_cache.TryGetValue(keywordName, out var list) || !list.Any())
				return null;

			IJsonSchemaKeyword keyword = null;
			var specials = list.Where(t => typeof(IJsonSchemaKeywordPlus).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
				.Select(t => (IJsonSchemaKeywordPlus) _resolver.Resolve(t))
				.ToList();
			if (specials.Any())
				keyword = specials.FirstOrDefault(k => k.Handles(json));

			if (keyword == null)
				keyword = (IJsonSchemaKeyword) _resolver.Resolve(list.First());
			keyword.FromJson(json, serializer);

			return keyword;
		}
	}
}
