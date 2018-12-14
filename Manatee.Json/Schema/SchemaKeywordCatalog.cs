using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Serialization;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Curates a list of all known JSON Schema keywords.
	/// </summary>
	public static class SchemaKeywordCatalog
	{
		private static readonly Dictionary<string, List<Type>> _cache = new Dictionary<string, List<Type>>();
		private static readonly ConstructorResolver _resolver = new ConstructorResolver();
		private static readonly Dictionary<string, SchemaVocabulary> _vocabularies = new Dictionary<string, SchemaVocabulary>();

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

		/// <summary>
		/// Adds a new keyword.
		/// </summary>
		/// <typeparam name="T">The type of the keyword implementation.</typeparam>
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
			if (!_vocabularies.TryGetValue(keyword.Vocabulary.Id, out var vocabulary))
			{
				vocabulary = keyword.Vocabulary;
				_vocabularies[keyword.Vocabulary.Id] = vocabulary;
			}

			if (!vocabulary.DefinedKeywords.Contains(typeof(T)))
				vocabulary.DefinedKeywords.Add(typeof(T));
		}
		/// <summary>
		/// Removes a keyword from use.
		/// </summary>
		/// <typeparam name="T">The type of the keyword implementation.</typeparam>
		public static void Remove<T>()
			where T : IJsonSchemaKeyword, new()
		{
			var keyword = (T) _resolver.Resolve(typeof(T));
			if (!_cache.TryGetValue(keyword.Name, out var list)) return;

			list.Remove(typeof(T));

			if (_vocabularies.TryGetValue(keyword.Vocabulary.Id, out var vocabulary))
			{
				vocabulary.DefinedKeywords.Remove(typeof(T));
				if (!vocabulary.DefinedKeywords.Any())
					_vocabularies.Remove(vocabulary.Id);
			}
		}

		internal static IJsonSchemaKeyword Build(string keywordName, JsonValue json, JsonSerializer serializer)
		{
			if (!_cache.TryGetValue(keywordName, out var list) || !list.Any())
				return null;

			IJsonSchemaKeyword keyword = null;
			var specials = list.Where(t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IJsonSchemaKeywordPlus)))
				.Select(t => (IJsonSchemaKeywordPlus) _resolver.Resolve(t))
				.ToList();
			if (specials.Any())
				keyword = specials.FirstOrDefault(k => k.Handles(json));

			if (keyword == null)
				keyword = (IJsonSchemaKeyword) _resolver.Resolve(list.First());
			keyword.FromJson(json, serializer);

			return keyword;
		}

		internal static SchemaVocabulary GetVocabulary(string id)
		{
			return _vocabularies.TryGetValue(id, out var vocabulary) ? vocabulary : null;

		}
	}
}
