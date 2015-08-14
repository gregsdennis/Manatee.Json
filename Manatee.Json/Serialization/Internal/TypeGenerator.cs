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
 
	File Name:		TypeGenerator.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		TypeGenerator
	Purpose:		Generates types at run-time which implement a given interface.

***************************************************************************************/

#if !IOS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Manatee.Json.Serialization.Internal
{
	internal static class TypeGenerator
	{
		private const string AssemblyName = "Manatee.Json.DynamicTypes";

		private static readonly AssemblyBuilder _assemblyBuilder;
		private static readonly ModuleBuilder _moduleBuilder;
		private static readonly Dictionary<Type, Type> _cache;

		static TypeGenerator()
		{
			var assemblyName = new AssemblyName(AssemblyName);
			// Note: To debug IL generation, please use the following line with your own test path.  Also need to uncomment the Save() call in the Generate<T>() method.
			//_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, @"E:\Projects\Manatee.Json\Manatee.Json.Tests\bin\Debug\");
#if NET35 || NET35C
			_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			_moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName);
#elif NET4 || NET4C || NET45
			_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			_moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName, AssemblyName + ".dll");
#endif
			_cache = new Dictionary<Type, Type>();
		}

		public static T Generate<T>()
		{
			var type = typeof (T);
			if (_cache.ContainsKey(type))
				return (T) ConstructInstance(_cache[type]);
			if (!type.IsInterface)
				throw new ArgumentException(string.Format("Type generation only works for interface types.  " +
				                                          "Type '{0}' is not valid.", type));
			var typeBuilder = CreateType(type);
			ImplementProperties<T>(typeBuilder);
			ImplementMethods<T>(typeBuilder);
			ImplementEvents<T>(typeBuilder);
			var concreteType = typeBuilder.CreateType();
			_cache.Add(type, concreteType);
			// Note: To debug IL generation, please uncomment the following line.  Also need to use the first _assemblyBuilder assignment in the static constructor.
			//_assemblyBuilder.Save(@"Manatee.Json.DynamicTypes.dll");
			return (T)ConstructInstance(concreteType);
		}

		private static TypeBuilder CreateType(Type type)
		{
			var typeBuilder = _moduleBuilder.DefineType("Concrete" + type.Name, TypeAttributes.Public);
			typeBuilder.AddInterfaceImplementation(type);
			return typeBuilder;
		}
		private static void ImplementProperties<T>(TypeBuilder builder)
		{
			var interfaceType = typeof (T);
			var properties = GetAllProperties(interfaceType);
			foreach (var propertyInfo in properties)
			{
				ImplementSingleProperty(builder, propertyInfo);
			}
		}
		private static IEnumerable<PropertyInfo> GetAllProperties(Type type)
		{
			var methods = new List<PropertyInfo>(type.GetProperties().Where(m => !m.IsSpecialName));
			var interfaceTypes = type.GetInterfaces();
			methods.AddRange(interfaceTypes.SelectMany(GetAllProperties));
			return methods;
		}
		private static void ImplementSingleProperty(TypeBuilder builder, PropertyInfo propertyInfo)
		{
			const MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.SpecialName |
			                                    MethodAttributes.HideBySig | MethodAttributes.Virtual;
			var fieldBuilder = builder.DefineField("_" + propertyInfo.Name, propertyInfo.PropertyType, FieldAttributes.Private);
			var indexers = propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray();
			var propertyBuilder = builder.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, indexers);
			MethodBuilder methodBuilder;
			if (propertyInfo.CanRead)
			{
				methodBuilder = builder.DefineMethod(propertyInfo.GetGetMethod().Name, methodAttr, propertyInfo.PropertyType, indexers);
				var il = methodBuilder.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, fieldBuilder);
				il.Emit(OpCodes.Ret);
				propertyBuilder.SetGetMethod(methodBuilder);
				builder.DefineMethodOverride(methodBuilder, propertyInfo.GetGetMethod());
			}
			if (propertyInfo.CanWrite)
			{
				methodBuilder = builder.DefineMethod(propertyInfo.GetGetMethod().Name, methodAttr, null,
				                                     indexers.Union(new[] {propertyInfo.PropertyType}).ToArray());
				var il = methodBuilder.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stfld, fieldBuilder);
				il.Emit(OpCodes.Ret);
				propertyBuilder.SetSetMethod(methodBuilder);
				builder.DefineMethodOverride(methodBuilder, propertyInfo.GetSetMethod());
			}
		}
		private static void ImplementMethods<T>(TypeBuilder builder)
		{
			var interfaceType = typeof(T);
			var methods = GetAllMethods(interfaceType);
			foreach (var methodInfo in methods)
			{
				ImplementSingleMethod(builder, methodInfo);
			}
		}
		private static IEnumerable<MethodInfo> GetAllMethods(Type type)
		{
			var methods = new List<MethodInfo>(type.GetMethods().Where(m => !m.IsSpecialName));
			var interfaceTypes = type.GetInterfaces();
			methods.AddRange(interfaceTypes.SelectMany(GetAllMethods));
			return methods;
		}
		private static void ImplementSingleMethod(TypeBuilder builder, MethodInfo methodInfo)
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
					var constraints = genericParameter.GetGenericParameterConstraints();
					foreach (var constraint in constraints)
					{
						if (constraint.IsInterface) typeParameter.SetInterfaceConstraints(constraint);
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
		private static void ImplementEvents<T>(TypeBuilder builder)
		{
			var interfaceType = typeof(T);
			var events = GetAllEvents(interfaceType);
			foreach (var eventInfo in events)
			{
				ImplementSingleEvent(builder, eventInfo);
			}
		}
		private static IEnumerable<EventInfo> GetAllEvents(Type type)
		{
			var events = new List<EventInfo>(type.GetEvents());
			var interfaceTypes = type.GetInterfaces();
			events.AddRange(interfaceTypes.SelectMany(GetAllEvents));
			return events;
		}
		private static void ImplementSingleEvent(TypeBuilder builder, EventInfo eventInfo)
		{
			const MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.NewSlot |
												MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final;
			var fieldBuilder = builder.DefineField("_" + eventInfo.Name, eventInfo.EventHandlerType, FieldAttributes.Private);
			var eventBuilder = builder.DefineEvent(eventInfo.Name, EventAttributes.None, eventInfo.EventHandlerType);

			var methodBuilder = builder.DefineMethod(eventInfo.GetAddMethod().Name, methodAttr, null,
			                                         new[] {eventInfo.EventHandlerType});
			var combineMethod = typeof (Delegate).GetMethod("Combine", new[] {typeof (Delegate), typeof (Delegate)});
			var removeMethod = typeof (Delegate).GetMethod("Remove", new[] {typeof (Delegate), typeof (Delegate)});
			var compareExchangeMethod = typeof (Interlocked).GetMethods()
			                                                .Single(m => (m.Name == "CompareExchange") && m.IsGenericMethod)
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
			builder.DefineMethodOverride(methodBuilder, eventInfo.GetAddMethod());

			methodBuilder = builder.DefineMethod(eventInfo.GetRemoveMethod().Name, methodAttr, null, new[] {eventInfo.EventHandlerType});
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
			builder.DefineMethodOverride(methodBuilder, eventInfo.GetRemoveMethod());
		}
		private static object ConstructInstance(Type type)
		{
			return Activator.CreateInstance(type, null);
		}
	}
}

#endif