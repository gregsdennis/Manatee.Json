/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonPathSyntaxException.cs
	Namespace:		Manatee.Json
	Class Name:		JsonPathSyntaxException
	Purpose:		Thrown when an input string contains a syntax error while
					parsing a <see cref="JsonPath"/>.

***************************************************************************************/
using System;
using JetBrains.Annotations;
using Manatee.Json.Path;

namespace Manatee.Json
{
	/// <summary>
	/// Thrown when an input string contains a syntax error while parsing a <see cref="JsonPath"/>.
	/// </summary>
	[Serializable]
	public class JsonPathSyntaxException : Exception
	{
		private readonly string _path;
		private readonly bool _isExpression;
		
		/// <summary>
		/// Gets the path up to the point at which the error was found.
		/// </summary>
		public string Path { get { return _path; } }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>
		/// The error message that explains the reason for the exception, or an empty string("").
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string Message { get { return string.Format(_isExpression ? "{0} Expression up to error: {1}" : "{0} Path up to error: {1}", base.Message, Path); } }

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
	}
}