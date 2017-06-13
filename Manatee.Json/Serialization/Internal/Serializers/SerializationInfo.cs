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