﻿/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSchemaPropertyValidatorFactory.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		JsonSchemaPropertyValidatorFactory
	Purpose:		Provides applicable validators.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal static class JsonSchemaPropertyValidatorFactory
	{
		private static readonly IEnumerable<IJsonSchemaPropertyValidator> AllValidators;

		static JsonSchemaPropertyValidatorFactory()
		{
			AllValidators = typeof(IJsonSchemaPropertyValidator).GetTypeInfo().Assembly.GetTypes()
														.Where(t => typeof(IJsonSchemaPropertyValidator).IsAssignableFrom(t) &&
																	!t.GetTypeInfo().IsAbstract &&
																	t.GetTypeInfo().IsClass)
			                                            .Select(Activator.CreateInstance)
			                                            .Cast<IJsonSchemaPropertyValidator>()
			                                            .ToList();
		}

		public static IEnumerable<IJsonSchemaPropertyValidator> Get(JsonSchema schema, JsonValue json)
		{
			return AllValidators.Where(v => v.Applies(schema, json));
		}
	}
}
