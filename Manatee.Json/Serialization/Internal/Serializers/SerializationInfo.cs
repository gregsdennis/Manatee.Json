using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class SerializationInfo
	{
		public MemberInfo MemberInfo { get; }
		public string SerializationName { get; }
		public bool ShouldTransform { get; }

	    public SerializationInfo(MemberInfo memberInfo, string serializationName, bool shouldTransform)
		{
			MemberInfo = memberInfo;
			SerializationName = serializationName;
		    ShouldTransform = shouldTransform;
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