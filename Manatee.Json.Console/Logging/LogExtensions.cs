// ==============================================================================
// 
// RealDimensions Software, LLC - Copyright © 2012 - Present - Released under the Apache 2.0 License
// 
// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
// ==============================================================================

using System;
using System.Collections.Concurrent;
using Manatee.Json.Console.Logging;

// ReSharper disable once CheckNamespace
namespace TacoPos.Logging
{
	/// <summary>
	/// Extensions to help make logging awesome - this should be installed into the root namespace of your application
	/// </summary>
	public static class LogExtensions
	{
		/// <summary>
		/// Concurrent dictionary that ensures only one instance of a logger for a type.
		/// </summary>
		private static readonly ConcurrentDictionary<Type, ILog> _dictionary = new ConcurrentDictionary<Type, ILog>();

		/// <summary>
		/// Gets the logger for <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">The type to get the logger for.</param>
		/// <returns>Instance of a logger for the object.</returns>
		public static ILog Log<T>(this T obj)
		{
			return Log(obj.GetType());
		}

		private static ILog Log(this Type type)
		{
			return _dictionary.GetOrAdd(type, Manatee.Json.Console.Logging.Log.GetLoggerFor);
		}
	}
}