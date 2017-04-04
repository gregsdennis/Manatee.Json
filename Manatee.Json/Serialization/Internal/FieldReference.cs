using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal class FieldReference : MemberReference<FieldInfo>
	{
		public override object GetValue(object instance)
		{
			return Info.GetValue(instance);
		}
		public override void SetValue(object instance, object value)
		{
			Info.SetValue(instance, value);
		}
	}
}