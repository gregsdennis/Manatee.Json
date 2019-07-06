using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Defines an index in a JSON Path array.
	/// </summary>
	public class Slice : IEquatable<Slice>
	{
		private int? _start;
		private int? _end;
		private int? _step;

		internal int? Index { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="Slice"/> class.
		/// </summary>
		/// <param name="index">A single index.</param>
		public Slice(int index)
		{
			Index = index;
		}
		/// <summary>
		/// Creates a new instance of the <see cref="Slice"/> class.
		/// </summary>
		/// <param name="start">The start index of the slice.</param>
		/// <param name="end">The end index of the slice.</param>
		/// <param name="step">Optional. The increment between each selected index between <paramref name="start"/> and <paramref name="end"/>.</param>
		public Slice(int? start, int? end, int? step = null)
		{
			_start = start;
			_end = end;
			_step = step;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return Index.HasValue
				       ? Index.ToString()
				       : _step.HasValue
					       ? $"{(_start.HasValue ? _start.ToString() : string.Empty)}:{(_end.HasValue ? _end.ToString() : string.Empty)}:{_step}"
					       : $"{(_start.HasValue ? _start.ToString() : string.Empty)}:{(_end.HasValue ? _end.ToString() : string.Empty)}";
		}
		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Slice other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Index == other.Index &&
			       _start == other._start &&
			       _end == other._end &&
			       _step == other._step;
		}
		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
		/// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		/// <param name="obj">The object to compare with the current object. </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			return Equals(obj as Slice);
		}
		/// <summary>Serves as a hash function for a particular type. </summary>
		/// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Index.GetHashCode();
				hashCode = (hashCode*397) ^ _start.GetHashCode();
				hashCode = (hashCode*397) ^ _end.GetHashCode();
				hashCode = (hashCode*397) ^ _step.GetHashCode();
				return hashCode;
			}
		}
		/// <summary>
		/// Implicit conversion from integer to a single-indexed <see cref="Slice"/>.
		/// </summary>
		/// <param name="i">The index.</param>
		public static implicit operator Slice(int i)
		{
			return new Slice(i);
		}

		internal IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			if (Index.HasValue)
			{
				return Index.Value < 0 || json.Count <= Index.Value
						   ? Enumerable.Empty<JsonValue>()
						   : new[] { json[Index.Value] };
			}

			var start = _ResolveIndex(_start ?? 0, json.Count);
			var end = _ResolveIndex(_end ?? json.Count, json.Count);
			var step = Math.Max(_step ?? 1, 1);

			// quick copy
			if (start == 0 && end == json.Count && step == 1)
				return new List<JsonValue>(json);

			if (step == 1)
			{
				var result = new JsonValue[end - start];
				json.CopyTo(start, result, 0, end - start);
				return result;
			}
			else
			{
				return _FindSlow(json, start, end, step);
			}
		}

		private static IEnumerable<JsonValue> _FindSlow(JsonArray json, int start, int end, int step)
		{
			var index = start;
			var list = new List<JsonValue>();
			while (index < end)
			{
				list.Add(json[index]);
				index += step;
			}
			return list;
		}

		private static int _ResolveIndex(int index, int count)
		{
			return index < 0 ? count + index : index;
		}
	}
}
