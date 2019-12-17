using System.Collections.Generic;

namespace Manatee.Json.Internal
{
	internal class MultiSetComparer<T> : IEqualityComparer<IEnumerable<T>>
	{
		private readonly IEqualityComparer<T> _comparer;

		public static MultiSetComparer<T> Default { get; } = new MultiSetComparer<T>();

		public MultiSetComparer(IEqualityComparer<T>? comparer = null)
		{
			_comparer = comparer ?? EqualityComparer<T>.Default;
		}

		public bool Equals(IEnumerable<T> first, IEnumerable<T> second)
		{
			if (first == null) return second == null;
			if (second == null) return false;
			if (ReferenceEquals(first, second)) return true;

			if (first is ICollection<T> firstCollection && second is ICollection<T> secondCollection)
			{
				if (firstCollection.Count != secondCollection.Count) return false;
				if (firstCollection.Count == 0) return true;
			}

			return !_HaveMismatchedElement(first, second);
		}

		private bool _HaveMismatchedElement(IEnumerable<T> first, IEnumerable<T> second)
		{
			var firstElementCounts = _GetElementCounts(first, out var firstNullCount);
			var secondElementCounts = _GetElementCounts(second, out var secondNullCount);

			if (firstNullCount != secondNullCount ||
			    firstElementCounts.Count != secondElementCounts.Count)
				return true;

			foreach (var kvp in firstElementCounts)
			{
				var firstElementCount = kvp.Value;
				secondElementCounts.TryGetValue(kvp.Key, out var secondElementCount);

				if (firstElementCount != secondElementCount) return true;
			}

			return false;
		}

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		private Dictionary<T, int> _GetElementCounts(IEnumerable<T> enumerable, out int nullCount)
		{
			var dictionary = new Dictionary<T, int>(_comparer);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
			nullCount = 0;

			foreach (var element in enumerable)
			{
				if (element == null)
					nullCount++;
				else
				{
					dictionary.TryGetValue(element, out var num);
					num++;
					dictionary[element] = num;
				}
			}

			return dictionary;
		}

		public int GetHashCode(IEnumerable<T> enumerable)
		{
			return enumerable.GetCollectionHashCode();
		}
	}
}