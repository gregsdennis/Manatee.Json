using System.Collections.Generic;

namespace Manatee.Json.Tests.Test_References
{
	public class ObjectWithExtendedProps : ObjectWithBasicProps
	{
		public ObjectWithExtendedProps LoopProperty { get; set; }
		public List<int> ListProperty { get; set; }
		public bool[] ArrayProperty { get; set; }

		#region Equality Testing

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ObjectWithExtendedProps)) return false;
			return Equals((ObjectWithExtendedProps)obj);
		}

		public bool Equals(ObjectWithExtendedProps other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && Equals(other.LoopProperty, LoopProperty);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ (LoopProperty != null ? LoopProperty.GetHashCode() : 0);
			}
		}
		#endregion
	}

	public class ObjectWithDuplicateProps
	{
		public ObjectWithBasicProps Prop1 { get; set; }
		public ObjectWithBasicProps Prop2 { get; set; }
	}
}
