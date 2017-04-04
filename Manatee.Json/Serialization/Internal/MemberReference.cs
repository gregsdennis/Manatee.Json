using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal abstract class MemberReference
	{
		public object Owner { get; set; }

		public abstract object GetValue(object instance);
		public abstract void SetValue(object instance, object value);
	}

	internal abstract class MemberReference<T> : MemberReference
		where T : MemberInfo
	{
		public T Info { get; set; }
	}
}