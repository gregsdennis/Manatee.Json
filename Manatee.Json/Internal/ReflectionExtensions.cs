using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Internal
{
	internal static class ReflectionExtensions
	{
		// Note: These methods assume that if a generic type is passed, the type is open.
		public static bool InheritsFrom(this Type tDerived, Type tBase)
		{
			if (tDerived == tBase) return true;
			if (tDerived._IsSubtypeOf(tBase)) return true;
			var interfaces = tDerived.GetTypeInfo().ImplementedInterfaces.SelectMany(_GetAllInterfaces);
			return interfaces.Contains(tBase);
		}
		private static IEnumerable<Type> _GetAllInterfaces(Type type)
		{
			if (type.GetTypeInfo().IsGenericType)
				yield return type.GetGenericTypeDefinition();
			yield return type;
		}
		private static bool _IsSubtypeOf(this Type tDerived, Type tBase)
		{
			var currentType = tDerived.GetTypeInfo().BaseType;
			while (currentType != null)
			{
				var typeInfo = currentType.GetTypeInfo();
				if (typeInfo.IsGenericType)
					currentType = currentType.GetGenericTypeDefinition();
				if (currentType == tBase) return true;
				currentType = typeInfo.BaseType;
			}
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetTypeArguments(this Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}
		public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo? type)
		{
			var properties = new List<PropertyInfo>();
			while (type != null)
			{
				properties.AddRange(type.DeclaredProperties);
				type = type.BaseType?.GetTypeInfo();
			}
			return properties;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumericType(this Type value)
		{
			return value == typeof(decimal) ||
			       value == typeof(double) ||
			       value == typeof(float) ||
			       value == typeof(int) ||
			       value == typeof(uint) ||
			       value == typeof(short) ||
			       value == typeof(ushort) ||
			       value == typeof(byte) ||
			       value == typeof(long) ||
			       value == typeof(ulong);
		}

		public static object? Default(this Type type)
		{
			return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
		}

		public static void SetMember(this object? obj, JsonPointer pointer, object? value)
		{
			while (true)
			{
				if (obj == null) return;
				if (pointer.Count == 0) throw new ArgumentException("Pointer must have at least one segment.");

				SerializationInfo member;
				var segment = pointer[0];
				if (int.TryParse(segment, out var index))
				{
					member = ReflectionCache.GetMembers(obj.GetType(), PropertySelectionStrategy.ReadWriteOnly, false)
						.SingleOrDefault(m => m.MemberInfo is PropertyInfo pi && pi.GetIndexParameters().Length == 1);
					if (member == null)
					{
						var jsonArray = (obj as JsonValue)?.Array ?? obj as JsonArray;
						if (jsonArray == null)
 						{
							return;
						}

						var segmentExists = jsonArray.Count > index;
						while (jsonArray.Count <= index)
						{
							jsonArray.Add(JsonValue.Null);
						}

						if (pointer.Count == 1)
						{
							JsonValue inferredValue = value is JsonValue jsonValue ? jsonValue :
								value is JsonObject jsonObjectValue ? jsonObjectValue :
								value is JsonArray jsonArrayValue ? jsonArrayValue :
								JsonValue.Null;

							jsonArray.RemoveAt(index);
							jsonArray.Insert(index, inferredValue);
							return;
						}

						JsonValue next;
						if (!segmentExists)
						{
							var nextSegment = pointer[1];
							if (int.TryParse(nextSegment, out int _))
								next = new JsonArray();
							else
								next = new JsonObject();
							
							jsonArray.RemoveAt(index);
							jsonArray.Insert(index, next);
						}
						else
						{
							next = jsonArray.ElementAt(index);
						}

						obj = next;
						pointer = new JsonPointer(pointer.Skip(1));
						continue;
					}

					var indexer = (PropertyInfo) member.MemberInfo;
					if (pointer.Count == 1)
					{
						indexer.SetValue(obj, value, new object[] {index});
						return;
					}

					var local = indexer.GetValue(obj, new object[] {index});
					obj = local;
					pointer = new JsonPointer(pointer.Skip(1));
					continue;
				}

				member = ReflectionCache.GetMembers(obj.GetType(), PropertySelectionStrategy.ReadWriteOnly, true)
					.SingleOrDefault(m => m.SerializationName == segment);
				if (member == null)
				{
					var jsonObject = (obj as JsonValue)?.Object ?? obj as JsonObject;
					if (jsonObject == null)
					{
						return;
					}

					var segmentExists = jsonObject.Keys.Contains(segment);
					if (pointer.Count == 1)
					{
						if (segmentExists)
						{
							jsonObject.Remove(segment);
						}

						JsonValue inferredValue = value is JsonValue jsonValue ? jsonValue :
							value is JsonObject jsonObjectValue ? jsonObjectValue :
							value is JsonArray jsonArrayValue ? jsonArrayValue :
							JsonValue.Null;

						jsonObject.Add(segment, inferredValue);
						return;
					}

					var local = jsonObject[segment];
					if (!segmentExists)
					{
						var nextSegment = pointer[1];
						if (int.TryParse(nextSegment, out int _))
							local = new JsonArray();
						else
							local = new JsonObject();

						jsonObject.Add(segment, local);
					}
					obj = local;
					pointer = new JsonPointer(pointer.Skip(1));
					continue;
				}

				if (member.MemberInfo is PropertyInfo asProperty)
				{
					if (pointer.Count == 1)
					{
						asProperty.SetValue(obj, value);
						return;
					}

					var local = asProperty.GetValue(obj);
					obj = local;
					pointer = new JsonPointer(pointer.Skip(1));
					continue;
				}

				if (member.MemberInfo is FieldInfo asField)
				{
					if (pointer.Count == 1)
					{
						asField.SetValue(obj, value);
						return;
					}

					var local = asField.GetValue(obj);
					obj = local;
					pointer = new JsonPointer(pointer.Skip(1));
					continue;
				}

				break;
			}
		}
		public static string CSharpName(this Type type)
		{
			var sb = new StringBuilder();
			type._CSharpName(sb);
			return sb.ToString();
		}

		private static void _CSharpName(this Type type, StringBuilder sb)
		{
			if (_TryCSharpKeyword(type, out var name))
			{
				sb.Append(name);
				return;
			}
			name = type.Name;
			bool comma;
			if (type.IsAnonymousType())
			{
				sb.Append("[anon]{");
				comma = false;
				foreach (var p in type.GetProperties())
				{
					if (comma)
						sb.Append(",");
					p.PropertyType._CSharpName(sb);
					comma = true;
				}
				sb.Append("}");
				return;
			}
			if (!type.IsGenericType)
			{
				sb.Append(type.Name);
				return;
			}

			sb.Append(name.Substring(0, name.IndexOf('`')));
			sb.Append("<");
			comma = false;
			foreach (var t in type.GetGenericArguments())
			{
				if (comma)
					sb.Append(",");
				t._CSharpName(sb);
				comma = true;
			}
			sb.Append(">");
		}
		// Source: https://stackoverflow.com/a/2483054/878701
		public static bool IsAnonymousType(this Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			// HACK: The only way to detect anonymous types right now.
			return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
			       && type.IsGenericType && type.Name.Contains("AnonymousType")
			       && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
			       && type.Attributes.HasFlag(TypeAttributes.NotPublic);
		}
		private static bool _TryCSharpKeyword(this Type type, [NotNullWhen(true)] out string? name)
		{
			if (type == typeof(string))
			{
				name = "string";
				return true;
			}
			if (type == typeof(byte))
			{
				name = "byte";
				return true;
			}
			if (type == typeof(sbyte))
			{
				name = "sbyte";
				return true;
			}
			if (type == typeof(short))
			{
				name = "short";
				return true;
			}
			if (type == typeof(ushort))
			{
				name = "ushort";
				return true;
			}
			if (type == typeof(int))
			{
				name = "int";
				return true;
			}
			if (type == typeof(uint))
			{
				name = "uint";
				return true;
			}
			if (type == typeof(long))
			{
				name = "long";
				return true;
			}
			if (type == typeof(ulong))
			{
				name = "ulong";
				return true;
			}
			if (type == typeof(float))
			{
				name = "float";
				return true;
			}
			if (type == typeof(double))
			{
				name = "double";
				return true;
			}
			if (type == typeof(char))
			{
				name = "char";
				return true;
			}
			if (type == typeof(decimal))
			{
				name = "decimal";
				return true;
			}
			if (type == typeof(bool))
			{
				name = "bool";
				return true;
			}
			if (type == typeof(object))
			{
				name = "object";
				return true;
			}

			name = null;
			return false;
		}
	}
}