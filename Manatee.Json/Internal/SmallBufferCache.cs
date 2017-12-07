using System;
using System.Collections.Generic;
using System.Text;

namespace Manatee.Json.Internal
{
    internal static class SmallBufferCache
    {
        static readonly int bufferSize = 8;

        static readonly ObjectCache<char[]> cache = new ObjectCache<char[]>(() => new char[bufferSize]);

        public static char[] Acquire(int size)
        {
            if (size <= bufferSize)
                return cache.Acquire();

            return new char[bufferSize];
        }

        public static void Release(char[] buffer)
        {
            if (buffer != null)
            {
                if (buffer.Length <= bufferSize)
                    cache.Release(buffer);
            }
        }
    }
}
