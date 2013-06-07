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
	Namespace:		Manatee.Json.Serialization
	Class Name:		TypeGenerator
	Purpose:		Generates types at run-time which implement a given interface.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Manatee.Json.Serialization.Internal
{
	internal class TypeGenerator
	{
		private const string AssemblyName = "Manatee.Json.DynamicTypes";

		private static readonly AssemblyBuilder _assemblyBuilder;
		private static readonly ModuleBuilder _moduleBuilder;
		private static readonly Dictionary<Type, Type> _cache;

		public static TypeGenerator Default { get; private set; }

		static TypeGenerator()
		{
			Default = new TypeGenerator();
			var assemblyName = new AssemblyName(AssemblyName);
			//_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, @"E:\Projects\Json\Json.Tests\bin\Debug\");
			_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			_moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName, AssemblyName + ".dll");
			_cache = new Dictionary<Type, Type>();
		}
		private TypeGenerator() {}

		public T Generate<T>()
		{
			var type = typeof (T);
			if (_cache.ContainsKey(type))
				return (T) ConstructInstance(_cache[type]);
			if (!type.IsInterface)
				throw new ArgumentException(string.Format("Type generation on works for interface types.  " +
				                                          "Type '{0}' is not valid.", type));
			var typeBuilder = CreateType(type);
			ImplementProperties<T>(typeBuilder);
			ImplementMethods<T>(typeBuilder);
			var concreteType = typeBuilder.CreateType();
			_cache.Add(type, concreteType);
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
		private static void ImplementSingleProperty(TypeBuilder builder, PropertyInfo property)
		{
			const MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.SpecialName |
			                                    MethodAttributes.HideBySig | MethodAttributes.Virtual;
			var fieldBuilder = builder.DefineField("_" + property.Name, property.PropertyType, FieldAttributes.Private);
			var indexers = property.GetIndexParameters().Select(p => p.ParameterType).ToArray();
			var propertyBuilder = builder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, indexers);
			MethodBuilder methodBuilder;
			if (property.CanRead)
			{
				methodBuilder = builder.DefineMethod(property.GetGetMethod().Name, methodAttr, property.PropertyType, indexers);
				var il = methodBuilder.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, fieldBuilder);
				il.Emit(OpCodes.Ret);
				propertyBuilder.SetGetMethod(methodBuilder);
				builder.DefineMethodOverride(methodBuilder, property.GetGetMethod());
			}
			if (property.CanWrite)
			{
				methodBuilder = builder.DefineMethod(property.GetGetMethod().Name, methodAttr, null,
				                                     indexers.Union(new[] {property.PropertyType}).ToArray());
				var il = methodBuilder.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Stfld, fieldBuilder);
				il.Emit(OpCodes.Ret);
				propertyBuilder.SetSetMethod(methodBuilder);
				builder.DefineMethodOverride(methodBuilder, property.GetSetMethod());
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
		private static void ImplementSingleMethod(TypeBuilder builder, MethodInfo method)
		{
			const MethodAttributes methodAttr = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final;
			var types = method.GetParameters().Select(p => p.ParameterType).ToArray();
			var methodBuilder = builder.DefineMethod(method.Name, methodAttr, method.ReturnType, types);
			if (method.ContainsGenericParameters)
			{
				var genericParameters = method.GetGenericArguments().Where(p => p.IsGenericParameter).ToList();
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
			var localBuilder = il.DeclareLocal(method.ReturnType);
			il.Emit(OpCodes.Ldloca_S, localBuilder);
			il.Emit(OpCodes.Initobj, localBuilder.LocalType);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ret);
		}
		private static object ConstructInstance(Type type)
		{
			return Activator.CreateInstance(type, null);
		}
	}
}