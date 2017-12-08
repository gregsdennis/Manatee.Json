using System;
using System.Threading;

namespace Manatee.Json.Internal
{
	internal class ObjectCache<T> where T : class
	{
		private static readonly int _cacheSize = Environment.ProcessorCount * 2;

		private readonly T[] _items = new T[_cacheSize];
		private T _item;

		private readonly Func<T> _builder;

		public ObjectCache(Func<T> builder)
		{
			_builder = builder;
		}

		public T Acquire()
		{
			var instance = _item;
			if (instance == null || instance != Interlocked.CompareExchange(ref _item, null, instance))
			{
				instance = _AcquireSlow();
			}

			return instance;
		}

		public void Release(T obj)
		{
			if (_item == null)
			{
				// Intentionally not using interlocked here. 
				// In a worst case scenario two objects may be stored into same slot.
				// It is very unlikely to happen and will only mean that one of the objects will get collected.
				_item = obj;
			}
			else
			{
				_ReleaseSlow(obj);
			}
		}

		private T _AcquireSlow()
		{
			for (int ii = 0; ii < _items.Length; ++ii)
			{
				// Note that the initial read is optimistically not synchronized. That is intentional. 
				// We will interlock only when we have a candidate. in a worst case we may miss some
				// recently returned objects. Not a big deal.
				var instance = _items[ii];
				if (instance != null)
				{
					if (instance == Interlocked.CompareExchange(ref _items[ii], null, instance))
					{
						return instance;
					}
				}
			}

			return _builder();
		}

		private void _ReleaseSlow(T obj)
		{
			for (int ii = 0; ii < _items.Length; ii++)
			{
				if (_items[ii] == null)
				{
					// Intentionally not using interlocked here. 
					// In a worst case scenario two objects may be stored into same slot.
					// It is very unlikely to happen and will only mean that one of the objects will get collected.
					_items[ii] = obj;
					break;
				}
			}
		}
	}
}
