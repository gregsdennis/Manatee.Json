/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ObjectWithBasicProps.cs
	Namespace:		Manatee.Tests.Test_References
	Class Name:		ObjectWithBasicProps
	Purpose:		Basic class that contains basic properties of various
					accessibility to be used in testing the Manatee.Json
					library.

***************************************************************************************/
using Manatee.Json.Attributes;
using Manatee.Json.Enumerations;

namespace Manatee.Tests.Test_References
{
	public class ObjectWithBasicProps
	{
		#region Serializable Instance Properties
		public string StringProp { get; set; }
		public int IntProp { get; set; }
		public double DoubleProp { get; set; }
		public bool BoolProp { get; set; }
		public JsonValueType EnumProp { get; set; }
		[JsonMapTo("MapToMe")]
		public int MappedProp { get; set; }
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
				   Equals(other.MappedProp, MappedProp);
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
				return result;
			}
		}
		#endregion
	}
}
