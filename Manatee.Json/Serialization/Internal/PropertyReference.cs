using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{

	internal class PropertyReference : MemberReference<PropertyInfo>
	{
		public override object GetValue(object instance)
		{
			return Info.GetValue(instance, null);
		}
		public override void SetValue(object instance, object value)
		{
			Info.SetValue(instance, value, null);
		}
	}
}
