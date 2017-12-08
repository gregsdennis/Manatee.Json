using System;
using System.Collections.Generic;
using System.Text;

namespace Manatee.Json.Internal
{
	internal static class SmallBufferCache
	{
		private const int _bufferSize = 8;

		private static readonly ObjectCache<char[]> _cache = new ObjectCache<char[]>(() => new char[_bufferSize]);

		public static char[] Acquire(int size)
		{
			if (size <= _bufferSize)
				return _cache.Acquire();

			return new char[_bufferSize];
		}

		public static void Release(char[] buffer)
		{
			if (buffer?.Length <= _bufferSize)
				_cache.Release(buffer);
		}
	}
}
