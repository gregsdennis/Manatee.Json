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
		private static readonly Dictionary<string, Type> _cache = new Dictionary<string, Type>();
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
			_cache[keyword.Name] = typeof(T);
		}

		internal static IJsonSchemaKeyword Build(string keywordName, JsonValue json, JsonSerializer serializer)
		{
			if (!_cache.TryGetValue(keywordName, out var type))
				throw new ArgumentException("Keyword not registered", keywordName);

			var keyword = (IJsonSchemaKeyword) _resolver.Resolve(type);
			keyword.FromJson(json, serializer);

			return keyword;

		}
	}
}
