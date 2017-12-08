using System;

namespace Manatee.Json.Internal
{
	internal static class SmallBufferCache
	{
		private const int _bufferSize = 8;
		private static readonly ObjectCache<char[]> _cache = new ObjectCache<char[]>(() => new char[_bufferSize]);

		public static char[] Acquire(int size)
		{
			return size <= _bufferSize
				       ? _cache.Acquire()
				       : new char[_bufferSize];
		}

		public static void Release(char[] buffer)
		{
			if (!(buffer?.Length <= _bufferSize)) return;

			Array.Clear(buffer, 0, buffer.Length);
			_cache.Release(buffer);
		}
	}
}
