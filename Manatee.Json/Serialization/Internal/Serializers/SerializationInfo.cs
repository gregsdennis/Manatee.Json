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
 
	File Name:		SerializationInfo.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		SerializationInfo
	Purpose:		Caches reflection-based serialization information.

***************************************************************************************/

using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SerializationInfo
	{
		public MemberInfo MemberInfo { get; }
		public string SerializationName { get; }

		public SerializationInfo(MemberInfo memberInfo, string serializationName)
		{
			MemberInfo = memberInfo;
			SerializationName = serializationName;
		}

		public override bool Equals(object obj)
		{
			return obj != null && MemberInfo.Equals(((SerializationInfo) obj).MemberInfo);
		}
		public override int GetHashCode()
		{
			return MemberInfo.GetHashCode();
		}
	}
}