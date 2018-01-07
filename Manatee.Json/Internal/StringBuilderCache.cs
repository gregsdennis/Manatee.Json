using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Manatee.Json.Internal
{
	internal static class StringBuilderCache
	{
		private static readonly ObjectCache<StringBuilder> cache = new ObjectCache<StringBuilder>(() => new StringBuilder());

		public static StringBuilder Acquire() => cache.Acquire();

		public static void Release(StringBuilder sb)
		{
			// Don't hold onto string builders that are too large
			if (sb.Capacity < 360)
			{
				sb.Clear();
				cache.Release(sb);
			}
		}

		public static string GetStringAndRelease(StringBuilder sb)
		{
			string result = sb.ToString();
			Release(sb);
			return result;
		}
	}
}
