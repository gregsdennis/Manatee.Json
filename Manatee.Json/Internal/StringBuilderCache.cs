using System.Text;

namespace Manatee.Json.Internal
{
	internal static class StringBuilderCache
	{
		private static readonly ObjectCache<StringBuilder> _cache = new ObjectCache<StringBuilder>(() => new StringBuilder());

		public static StringBuilder Acquire() => _cache.Acquire();

		public static void Release(StringBuilder sb)
		{
			// Don't hold onto string builders that are too large
			if (sb.Capacity >= 360) return;

			sb.Clear();
			_cache.Release(sb);
		}

		public static string GetStringAndRelease(StringBuilder sb)
		{
			var result = sb.ToString();
			Release(sb);
			return result;
		}
	}
}
