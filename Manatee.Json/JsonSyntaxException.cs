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
 
	File Name:		JsonSyntaxException.cs
	Namespace:		Manatee.Json
	Class Name:		JsonSyntaxException
	Purpose:		Thrown when an input string contains a syntax error.

***************************************************************************************/

using System;
using JetBrains.Annotations;
using Manatee.Json.Path;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains a syntax error.
	/// </summary>
	[Serializable]
	public class JsonSyntaxException : Exception
	{
		private string _path;

		public string Path { get { return string.Format("${0}", _path); } }

		[StringFormatMethod("format")]
		internal JsonSyntaxException(string format, params object[] parameters)
			: base(string.Format(format, parameters)) { }

		public void PrependPath(string part)
		{
			_path = part + _path;
		}

		public override string ToString()
		{
			return string.Format("{0} Path: '{1}'.\n{2}", Message, Path, StackTrace);
		}
	}
	/// <summary>
	/// Thrown when an input string contains a syntax error.
	/// </summary>
	[Serializable]
	public class JsonPathSyntaxException : Exception
	{
		private readonly string _path;
		private readonly bool _isExpression;

		public string Path { get { return _path; } }

		[StringFormatMethod("format")]
		internal JsonPathSyntaxException(JsonPath path, string format, params object[] parameters)
			: base(string.Format(format, parameters))
		{
			_path = path.ToString();
		}
		[StringFormatMethod("format")]
		internal JsonPathSyntaxException(string expression, string format, params object[] parameters)
			: base(string.Format(format, parameters))
		{
			_isExpression = true;
			_path = expression;
		}

		public override string ToString()
		{
			return _isExpression
				? string.Format("{0} Expression up to error: '{1}'.\n{2}", Message, Path, StackTrace)
				: string.Format("{0} Path up to error: '{1}'.\n{2}", Message, Path, StackTrace);
		}
	}
}