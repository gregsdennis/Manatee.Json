using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Manatee.Json.Internal
{
	class ObjectCache<T> where T : class
	{
		private static readonly int CACHE_SIZE = Environment.ProcessorCount * 2;

		private T item = null;
		private readonly T[] items = new T[CACHE_SIZE];

		private readonly Func<T> builder;

		public ObjectCache(Func<T> builder)
		{
			this.builder = builder;
		}

		public T Acquire()
		{
			var instance = item;
			if (instance == null || instance != Interlocked.CompareExchange(ref item, null, instance))
			{
				instance = AcquireSlow();
			}

			return instance;
		}

		private T AcquireSlow()
		{
			for (int ii = 0; ii < items.Length; ++ii)
			{
				// Note that the initial read is optimistically not synchronized. That is intentional. 
				// We will interlock only when we have a candidate. in a worst case we may miss some
				// recently returned objects. Not a big deal.
				var instance = items[ii];
				if (instance != null)
				{
					if (instance == Interlocked.CompareExchange(ref items[ii], null, instance))
					{
						return instance;
					}
				}
			}

			return builder();
		}

		public void Release(T obj)
		{
			if (item == null)
			{
				// Intentionally not using interlocked here. 
				// In a worst case scenario two objects may be stored into same slot.
				// It is very unlikely to happen and will only mean that one of the objects will get collected.
				item = obj;
			}
			else
			{
				ReleaseSlow(obj);
			}
		}

		private void ReleaseSlow(T obj)
		{
			for (int ii = 0; ii < items.Length; ii++)
			{
				if (items[ii] == null)
				{
					// Intentionally not using interlocked here. 
					// In a worst case scenario two objects may be stored into same slot.
					// It is very unlikely to happen and will only mean that one of the objects will get collected.
					items[ii] = obj;
					break;
				}
			}
		}
	}
}
