using System;
using JetBrains.Annotations;
using DebugLog = System.Console;

namespace Manatee.Json.Console.Logging
{
	/// <summary>
	/// The default logger until one is set.
	/// </summary>
	public class NullLog : ILog, ILog<NullLog>
	{
		public void InitializeFor(Type loggerName)
		{
		}

		[StringFormatMethod("message")]
		public void Trace(string message, params object[] formatting)
		{
			DebugLog.WriteLine($"DEBUG: {message}", formatting);
		}

		public void Trace(Func<string> message)
		{
			DebugLog.WriteLine($"DEBUG: {message()}");
		}

		[StringFormatMethod("message")]
		public void Debug(string message, params object[] formatting)
		{
			DebugLog.WriteLine($"DEBUG: {message}", formatting);
		}

		public void Debug(Func<string> message)
		{
			DebugLog.WriteLine($"DEBUG: {message()}");
		}

		[StringFormatMethod("message")]
		public void Info(string message, params object[] formatting)
		{
			DebugLog.WriteLine($"{message}", formatting);
		}

		public void Info(Func<string> message)
		{
			DebugLog.WriteLine($"{message()}");
		}

		[StringFormatMethod("message")]
		public void Warn(string message, params object[] formatting)
		{
			DebugLog.WriteLine($"WARN: {message}", formatting);
		}

		public void Warn(Func<string> message)
		{
			DebugLog.WriteLine($"WARN: {message()}");
		}

		[StringFormatMethod("message")]
		public void Error(string message, params object[] formatting)
		{
			DebugLog.WriteLine($"ERROR: {message}", formatting);
		}

		public void Error(Func<string> message)
		{
			DebugLog.WriteLine($"ERROR: {message()}");
		}

		public void Error(Func<string> message, Exception exception)
		{
			DebugLog.WriteLine($"ERROR: {message()}\n{exception.ExtendedMessage()}");
		}

		[StringFormatMethod("message")]
		public void Fatal(string message, params object[] formatting)
		{
			DebugLog.WriteLine($"FATAL: {message}", formatting);
		}

		public void Fatal(Func<string> message)
		{
			DebugLog.WriteLine($"FATAL: {message()}");
		}

		public void Fatal(Func<string> message, Exception exception)
		{
			DebugLog.WriteLine($"FATAL: {message()}\n{exception.ExtendedMessage()}");
		}
	}
}