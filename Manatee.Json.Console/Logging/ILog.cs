using System;
using JetBrains.Annotations;

namespace Manatee.Json.Console.Logging
{
	/// <summary>
	/// Custom interface for logging messages
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Initializes the instance for the logger name
		/// </summary>
		/// <param name="loggerType">Type of the logger</param>
		void InitializeFor(Type loggerType);

		/// <summary>
		/// Trace level of the specified message. The other method is preferred since the execution is deferred.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatting">The formatting.</param>
		[StringFormatMethod("message")]
		void Trace(string message, params object[] formatting);

		/// <summary>
		/// Trace level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Trace(Func<string> message);

		/// <summary>
		/// Debug level of the specified message. The other method is preferred since the execution is deferred.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatting">The formatting.</param>
		[StringFormatMethod("message")]
		void Debug(string message, params object[] formatting);

		/// <summary>
		/// Debug level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Debug(Func<string> message);

		/// <summary>
		/// Info level of the specified message. The other method is preferred since the execution is deferred.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatting">The formatting.</param>
		[StringFormatMethod("message")]
		void Info(string message, params object[] formatting);

		/// <summary>
		/// Info level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Info(Func<string> message);

		/// <summary>
		/// Warn level of the specified message. The other method is preferred since the execution is deferred.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatting">The formatting.</param>
		[StringFormatMethod("message")]
		void Warn(string message, params object[] formatting);

		/// <summary>
		/// Warn level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Warn(Func<string> message);

		/// <summary>
		/// Error level of the specified message. The other method is preferred since the execution is deferred.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatting">The formatting.</param>
		[StringFormatMethod("message")]
		void Error(string message, params object[] formatting);

		/// <summary>
		/// Error level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Error(Func<string> message);

		/// <summary>
		/// Error level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		void Error(Func<string> message, Exception exception);

		/// <summary>
		/// Fatal level of the specified message. The other method is preferred since the execution is deferred.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatting">The formatting.</param>
		[StringFormatMethod("message")]
		void Fatal(string message, params object[] formatting);

		/// <summary>
		/// Fatal level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Fatal(Func<string> message);

		/// <summary>
		/// Fatal level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="exception">The exception.</param>
		void Fatal(Func<string> message, Exception exception);
	}

	/// <summary>
	/// Ensures a default constructor for the logger type
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ILog<T> //where T : new()
	{
	}
}