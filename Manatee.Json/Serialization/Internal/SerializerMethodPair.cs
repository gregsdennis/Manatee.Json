using System;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializerMethodPair
	{
		public Func<JsonSerializer, object, object> Serializer { get; }

		public Func<JsonSerializer, object, object> Deserializer { get; }

		public SerializerMethodPair(Type type)
		{
			Serializer = _MakeTypedSerializer(type);
			Deserializer = _MakeTypedDeserializer(type);
		}

		private static Func<JsonSerializer, object, object> _MakeTypedSerializer(Type type)
		{
			var serializer = _GetTypedSerializeMethod(type);
			return _MakeDelegate<JsonSerializer>(serializer);
		}
		private static Func<JsonSerializer, object, object> _MakeTypedDeserializer(Type type)
		{
			var deserializer = _GetTypedDeserializeMethod(type);
			return _MakeDelegate<JsonSerializer>(deserializer);
		}
		private static MethodInfo _GetTypedSerializeMethod(Type type)
		{
			return typeof(JsonSerializer).GetTypeInfo().GetDeclaredMethod(nameof(JsonSerializer.Serialize))
										 .MakeGenericMethod(type);
		}
		private static MethodInfo _GetTypedDeserializeMethod(Type type)
		{
			return typeof(JsonSerializer).GetTypeInfo().GetDeclaredMethod(nameof(JsonSerializer.Deserialize))
										 .MakeGenericMethod(type);
		}
		// adapted from: https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
		private static Func<TInstance, object, object> _MakeDelegate<TInstance>(MethodInfo methodInfo)
		{
			var helper = typeof(SerializerMethodPair).GetTypeInfo()
													 .GetDeclaredMethod(nameof(_MakeDelegateHelper));

			// Get a concrete instance of our helper method
			var realHelper = helper.MakeGenericMethod(
				typeof(TInstance),
				methodInfo.GetParameters()[0].ParameterType,
				methodInfo.ReturnType);

			// Apply the helper method to our method
			var method = realHelper.Invoke(null, new object[] { methodInfo });

			return (Func<TInstance, object, object>)method;
		}
		private static Func<TInstance, object, object> _MakeDelegateHelper<TInstance, TParam, TReturn>(MethodInfo methodInfo)
			where TInstance : class
		{
			Func<TInstance, TParam, TReturn> method
				= (Func<TInstance, TParam, TReturn>)methodInfo.CreateDelegate(typeof(Func<TInstance, TParam, TReturn>));

			Func<TInstance, object, object> methodCaller
				= (TInstance instance, object param) => method(instance, (TParam)param);
			return methodCaller;
		}
	}
}
