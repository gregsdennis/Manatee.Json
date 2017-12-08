using System;

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
			{
				Array.Clear(buffer, 0, buffer.Length);
				_cache.Release(buffer);
			}
		}
	}
}
