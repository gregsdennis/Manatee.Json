using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Manatee.Json.Serialization.Internal
{
	internal static class TypeGenerator
	{
		private const string AssemblyName = "Manatee.Json.DynamicTypes";

		private static readonly AssemblyBuilder _assemblyBuilder;
		private static readonly ModuleBuilder _moduleBuilder;
		private static readonly Dictionary<Type, TypeInfo> _cache;

		static TypeGenerator()
		{
			var assemblyName = new AssemblyName(AssemblyName);
			_assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			_moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName + ".dll");
			_cache = new Dictionary<Type, TypeInfo>();
		}

		public static T Generate<T>()
		{
			var type = typeof (T);
			if (!_cache.TryGetValue(type, out TypeInfo concreteType))
			{
				var typeInfo = type.GetTypeInfo();
				if (!typeInfo.IsInterface)
					throw new ArgumentException($"Type generation only works for interface types. Type '{type}' is not an interface.");
				if (!typeInfo.IsPublic)
				{
					var assembly = typeInfo.Assembly;
					var internalsVisible = assembly.GetCustomAttributes<InternalsVisibleToAttribute>()
												   .Any(a => a.AssemblyName == AssemblyName);
					if (!internalsVisible)
						throw new ArgumentException($"Type generation only works for accessible interface types. Type '{type}' is not accessible. " +
													$"If possible, make the type public or add '[assembly:InternalsVisibleTo(\"{AssemblyName}\")] " +
													$"to assembly '{assembly.FullName}'.");
				}
				var typeBuilder = _CreateTypeBuilder(type);
				_ImplementProperties<T>(typeBuilder);
				_ImplementMethods<T>(typeBuilder);
				_ImplementEvents<T>(typeBuilder);
				concreteType = typeBuilder.CreateTypeInfo();
				_cache.Add(type, concreteType);
			}
			return (T) _ConstructInstance(concreteType.AsType());
		}

		private static TypeBuilder _CreateTypeBuilder(Type type)
		{
			var typeBuilder = _moduleBuilder.DefineType("Concrete" + type.Name, TypeAttributes.Public);
			typeBuilder.AddInterfaceImplementation(type);
			return typeBuilder;
		}
		private static void _ImplementProperties<T>(TypeBuilder builder)
		{
			var interfaceType = typeof (T);
			var properties = _GetAllProperties(interfaceType);
			foreach (var propertyInfo in properties)
			{
				_ImplementSingleProperty(builder, propertyInfo);
			}
		}
		private static IEnumerable<PropertyInfo> _GetAllProperties(Type type)
		{
			var methods = new List<PropertyInfo>(type.GetTypeInfo().DeclaredProperties.Where(m => !m.IsSpecialName));
			var interfaceTypes = type.GetTypeInfo().ImplementedInterfaces;
			methods.AddRange(interfaceTypes.SelectMany(_GetAllProperties));
			return methods;
		}
		private static void _ImplementSingleProperty(TypeBuilder builder, PropertyInfo propertyInfo)
		{
			const MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.SpecialName |
			                                    MethodAttributes.HideBySig | MethodAttributes.Virtual;
			var fieldBuilder = builder.DefineField("_" + propertyInfo.Name, propertyInfo.PropertyType, FieldAttributes.Private);
			var indexers = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();
			var propertyBuilder = builder.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, indexers);
			MethodBuilder methodBuilder;
			if (propertyInfo.CanRead)
			{
				methodBuilder = builder.DefineMethod(propertyInfo.GetMethod.Name, methodAttr, propertyInfo.PropertyType, indexers);
				var il = methodBuilder.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, fieldBuilder);
				il.Emit(OpCodes.Ret);
				propertyBuilder.SetGetMethod(methodBuilder);
				builder.DefineMethodOverride(methodBuilder, propertyInfo.GetMethod);
			}
			if (propertyInfo.CanWrite)
			{
				methodBuilder = builder.DefineMethod(propertyInfo.GetMethod.Name, methodAttr, null,
				                                     indexers.Union(new[] {propertyInfo.PropertyType}).ToArray());
				var il = methodBuilder.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stfld, fieldBuilder);
				il.Emit(OpCodes.Ret);
				propertyBuilder.SetSetMethod(methodBuilder);
				builder.DefineMethodOverride(methodBuilder, propertyInfo.SetMethod);
			}
		}
		private static void _ImplementMethods<T>(TypeBuilder builder)
		{
			var interfaceType = typeof(T);
			var methods = _GetAllMethods(interfaceType);
			foreach (var methodInfo in methods)
			{
				_ImplementSingleMethod(builder, methodInfo);
			}
		}
		private static IEnumerable<MethodInfo> _GetAllMethods(Type type)
		{
			var methods = new List<MethodInfo>(type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsSpecialName));
			var interfaceTypes = type.GetTypeInfo().ImplementedInterfaces;
			methods.AddRange(interfaceTypes.SelectMany(_GetAllMethods));
			return methods;
		}
		private static void _ImplementSingleMethod(TypeBuilder builder, MethodInfo methodInfo)
		{
			const MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final;
			var types = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
			var methodBuilder = builder.DefineMethod(methodInfo.Name, methodAttr, methodInfo.ReturnType, types);
			if (methodInfo.ContainsGenericParameters)
			{
				var genericParameters = methodInfo.GetGenericArguments().Where(p => p.IsGenericParameter).ToList();
				var names = genericParameters.Select(p => p.Name).ToArray();
				var typeParameters = methodBuilder.DefineGenericParameters(names);
				foreach (var typeParameter in typeParameters)
				{
					var genericParameter = genericParameters.Single(p => p.Name == typeParameter.Name);
					var constraints = genericParameter.GetTypeInfo().GetGenericParameterConstraints();
					foreach (var constraint in constraints)
					{
						if (constraint.GetTypeInfo().IsInterface) typeParameter.SetInterfaceConstraints(constraint);
						else typeParameter.SetBaseTypeConstraint(constraint);
					}
				}
			}
			var il = methodBuilder.GetILGenerator();
			var localBuilder = il.DeclareLocal(methodInfo.ReturnType);
			il.Emit(OpCodes.Ldloca_S, localBuilder);
			il.Emit(OpCodes.Initobj, localBuilder.LocalType);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ret);
		}
		private static void _ImplementEvents<T>(TypeBuilder builder)
		{
			var interfaceType = typeof(T);
			var events = _GetAllEvents(interfaceType);
			foreach (var eventInfo in events)
			{
				_ImplementSingleEvent(builder, eventInfo);
			}
		}
		private static IEnumerable<EventInfo> _GetAllEvents(Type type)
		{
			var events = new List<EventInfo>(type.GetTypeInfo().DeclaredEvents);
			var interfaceTypes = type.GetTypeInfo().ImplementedInterfaces;
			events.AddRange(interfaceTypes.SelectMany(_GetAllEvents));
			return events;
		}
		private static void _ImplementSingleEvent(TypeBuilder builder, EventInfo eventInfo)
		{
			const MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.NewSlot |
												MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final;
			var fieldBuilder = builder.DefineField("_" + eventInfo.Name, eventInfo.EventHandlerType, FieldAttributes.Private);
			var eventBuilder = builder.DefineEvent(eventInfo.Name, EventAttributes.None, eventInfo.EventHandlerType);

			var methodBuilder = builder.DefineMethod(eventInfo.AddMethod.Name, methodAttr, null,
			                                         new[] {eventInfo.EventHandlerType});
			var combineMethod = typeof (Delegate).GetRuntimeMethod("Combine", new[] {typeof (Delegate), typeof (Delegate)});
			var removeMethod = typeof (Delegate).GetRuntimeMethod("Remove", new[] {typeof (Delegate), typeof (Delegate)});
			var compareExchangeMethod = typeof(Interlocked).GetTypeInfo().DeclaredMethods
			                                               .Single(m => m.Name == "CompareExchange" && m.IsGenericMethod)
			                                               .MakeGenericMethod(eventInfo.EventHandlerType);
			var il = methodBuilder.GetILGenerator();
			il.DeclareLocal(eventInfo.EventHandlerType);
			il.DeclareLocal(eventInfo.EventHandlerType);
			il.DeclareLocal(eventInfo.EventHandlerType);
			il.DeclareLocal(typeof (bool));
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldBuilder);
			il.Emit(OpCodes.Stloc_0);
			var label = il.DefineLabel();
			il.MarkLabel(label);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Stloc_1);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Call, combineMethod);
			il.Emit(OpCodes.Castclass, eventInfo.EventHandlerType);
			il.Emit(OpCodes.Stloc_2);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldflda, fieldBuilder);
			il.Emit(OpCodes.Ldloc_2);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Call, compareExchangeMethod);
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Ceq);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ceq);
			il.Emit(OpCodes.Stloc_3);
			il.Emit(OpCodes.Ldloc_3);
			il.Emit(OpCodes.Brtrue_S, label);
			il.Emit(OpCodes.Ret);
			eventBuilder.SetAddOnMethod(methodBuilder);
			builder.DefineMethodOverride(methodBuilder, eventInfo.AddMethod);

			methodBuilder = builder.DefineMethod(eventInfo.RemoveMethod.Name, methodAttr, null, new[] {eventInfo.EventHandlerType});
			il = methodBuilder.GetILGenerator();
			il.DeclareLocal(eventInfo.EventHandlerType);
			il.DeclareLocal(eventInfo.EventHandlerType);
			il.DeclareLocal(eventInfo.EventHandlerType);
			il.DeclareLocal(typeof (bool));
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldBuilder);
			il.Emit(OpCodes.Stloc_0);
			label = il.DefineLabel();
			il.MarkLabel(label);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Stloc_1);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Call, removeMethod);
			il.Emit(OpCodes.Castclass, eventInfo.EventHandlerType);
			il.Emit(OpCodes.Stloc_2);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldflda, fieldBuilder);
			il.Emit(OpCodes.Ldloc_2);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Call, compareExchangeMethod);
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Ceq);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ceq);
			il.Emit(OpCodes.Stloc_3);
			il.Emit(OpCodes.Ldloc_3);
			il.Emit(OpCodes.Brtrue_S, label);
			il.Emit(OpCodes.Ret);
			eventBuilder.SetRemoveOnMethod(methodBuilder);
			builder.DefineMethodOverride(methodBuilder, eventInfo.RemoveMethod);
		}
		private static object _ConstructInstance(Type type)
		{
			return Activator.CreateInstance(type, null);
		}
	}
}
