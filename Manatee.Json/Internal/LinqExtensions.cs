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
	}
}