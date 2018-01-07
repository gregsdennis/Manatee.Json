using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Manatee.Json.Serialization;

namespace Manatee.Json.Tests.Test_References
{
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
	public class ObjectWithBasicProps
	{
		public string Field;
		public readonly string ReadOnlyField = "read only";

		#region Serializable Instance Properties
		public string StringProp { get; set; }
		public int IntProp { get; set; }
		public double DoubleProp { get; set; }
		public bool BoolProp { get; set; }
		public TestEnum EnumProp { get; set; }
		public FlagsEnum FlagsEnumProp { get; set; }
		[JsonMapTo("MapToMe")]
		public int MappedProp { get; set; }
		public List<int> ReadOnlyListProp { get; } = new List<int>();
		public Dictionary<string, int> ReadOnlyDictionaryProp { get; } = new Dictionary<string, int>();
		#endregion

		#region Nonserializable Instance Properties
		public string ReadOnlyProp { get; private set; }
		public string WriteOnlyProp { private get; set; }
		[JsonIgnore]
		public string IgnoreProp { get; set; }
		#endregion

		#region Serializable Static Properties
		public static string StaticStringProp { get; set; }
		public static int StaticIntProp { get; set; }
		public static double StaticDoubleProp { get; set; }
		public static bool StaticBoolProp { get; set; }
		#endregion

		#region Nonserializable Static Properties
		public static string StaticReadOnlyProp { get; private set; }
		public static string StaticWriteOnlyProp { private get; set; }
		[JsonIgnore]
		public static string StaticIgnoreProp { get; set; }
		#endregion

		#region Equality Testing
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(ObjectWithBasicProps)) return false;
			return Equals((ObjectWithBasicProps)obj);
		}
		public bool Equals(ObjectWithBasicProps other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.StringProp, StringProp) &&
			       other.IntProp == IntProp &&
			       other.DoubleProp.Equals(DoubleProp) &&
			       other.BoolProp.Equals(BoolProp) &&
			       Equals(other.ReadOnlyProp, ReadOnlyProp) &&
			       Equals(other.WriteOnlyProp, WriteOnlyProp) &&
			       Equals(other.IgnoreProp, IgnoreProp) &&
				   Equals(other.EnumProp, EnumProp) &&
				   Equals(other.MappedProp, MappedProp) &&
				   Equals(other.Field, Field);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int result = (StringProp != null ? StringProp.GetHashCode() : 0);
				result = (result * 397) ^ IntProp;
				result = (result * 397) ^ DoubleProp.GetHashCode();
				result = (result * 397) ^ BoolProp.GetHashCode();
				result = (result * 397) ^ (ReadOnlyProp != null ? ReadOnlyProp.GetHashCode() : 0);
				result = (result * 397) ^ (WriteOnlyProp != null ? WriteOnlyProp.GetHashCode() : 0);
				result = (result * 397) ^ (IgnoreProp != null ? IgnoreProp.GetHashCode() : 0);
				result = (result * 397) ^ EnumProp.GetHashCode();
				result = (result * 397) ^ MappedProp;
				result = (result * 397) ^ Field?.GetHashCode() ?? 0;
				return result;
			}
		}
		#endregion
	}

	public enum TestEnum
	{
		None,
		BasicEnumValue,
		[Display(Description = "enum_value_with_description")]
		EnumValueWithDescription
	}

	[Flags]
	public enum FlagsEnum
	{
		None = 0,
		BasicEnumValue = 1,
		[Display(Description = "enum_value_with_description")]
		EnumValueWithDescription = 2
	}
// ReSharper restore UnusedAutoPropertyAccessor.Local
// ReSharper restore UnusedMember.Global
}
