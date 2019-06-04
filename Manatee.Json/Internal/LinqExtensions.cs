using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
	internal static class LinqExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> items)
			where T : class
		{
			return items.Where(i => i != null);
		}

		public static bool ContentsEqual<T>(this IEnumerable<T> a, IEnumerable<T> b)
		{
			if (a == null && b != null) return false;
			if (a != null && b == null) return false;
			if (a == null) return true;

			var listA = a.ToList();
			var listB = b.ToList();
			return listA.Count == listB.Count && listA.All(item => listB.Contains(item));
		}

		private static IEnumerable<TOut> _LeftJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
		                                                               IEnumerable<TB> bItems,
		                                                               Func<TA, TKey> aKeySelector,
		                                                               Func<TB, TKey> bKeySelector,
		                                                               Func<TA, TB, TOut> selector)
		{
			return aItems.GroupJoin(bItems,
			                        aKeySelector,
			                        bKeySelector,
			                        (a, bSet) => new {a, bSet})
				.SelectMany(t => t.bSet.DefaultIfEmpty(),
				            (t, b) => selector(t.a, b));
		}

		private static IEnumerable<TOut> _LeftJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
		                                                               IEnumerable<TB> bItems,
		                                                               Func<TA, TKey> aKeySelector,
		                                                               Func<TB, TKey> bKeySelector,
		                                                               Func<TA, TB, TOut> selector,
		                                                               IEqualityComparer<TKey> comparer)
		{
			return aItems.GroupJoin(bItems,
			                        aKeySelector,
			                        bKeySelector,
			                        (a, bSet) => new {a, bSet},
			                        comparer)
				.SelectMany(t => t.bSet.DefaultIfEmpty(),
				            (t, b) => selector(t.a, b));
		}

		public static IEnumerable<TOut> FullOuterJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
		                                                                  IEnumerable<TB> bItems,
		                                                                  Func<TA, TKey> aKeySelector,
		                                                                  Func<TB, TKey> bKeySelector,
		                                                                  Func<TA, TB, TOut> selector)
		{
			var fromA = _LeftJoin(aItems,
			                      bItems,
			                      aKeySelector,
			                      bKeySelector,
			                      (a, b) => new {a, b});
			var fromB = _LeftJoin(bItems,
			                      aItems,
			                      bKeySelector,
			                      aKeySelector,
			                      (b, a) => new {a, b});

			return fromA.Union(fromB).Select(x => selector(x.a, x.b));
		}

		public static IEnumerable<TOut> FullOuterJoin<TA, TB, TKey, TOut>(this IEnumerable<TA> aItems,
		                                                                  IEnumerable<TB> bItems,
		                                                                  Func<TA, TKey> aKeySelector,
		                                                                  Func<TB, TKey> bKeySelector,
		                                                                  Func<TA, TB, TOut> selector,
		                                                                  IEqualityComparer<TKey> comparer)
		{
			var fromA = _LeftJoin(aItems,
			                      bItems,
			                      aKeySelector,
			                      bKeySelector,
			                      (a, b) => new {a, b},
			                      comparer);
			var fromB = _LeftJoin(bItems,
			                      aItems,
			                      bKeySelector,
			                      aKeySelector,
			                      (b, a) => new {a, b},
			                      comparer);

			return fromA.Union(fromB).Select(x => selector(x.a, x.b));
		}

		public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			return items.Select((item, i) => new {Item = item, Index = i})
				.Where(i => predicate(i.Item))
				.Select(i => i.Index);
		}
	}
}