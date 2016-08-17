/***************************************************************************************

	Copyright 2016 Greg Dennis

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
