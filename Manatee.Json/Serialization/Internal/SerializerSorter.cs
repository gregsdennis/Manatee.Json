using System.Collections.Generic;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializerSorter : IComparer<ISerializer>
	{
		public static SerializerSorter Instance { get; } = new SerializerSorter();

		private SerializerSorter() { }

		public int Compare(ISerializer x, ISerializer y)
		{
			var prioritizedX = x as IPrioritizedSerializer;
			var prioritizedy = y as IPrioritizedSerializer;

			var xPriority = prioritizedX?.Priority ?? 0;
			var yPriority = prioritizedy?.Priority ?? 0;

			return yPriority - xPriority;
		}
	}
}
