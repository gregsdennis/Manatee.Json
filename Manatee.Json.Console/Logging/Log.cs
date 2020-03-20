using System;

namespace Manatee.Json.Console.Logging
{
	/// <summary>
	/// Logger type initialization
	/// </summary>
	public static class Log
	{
		private static Type _logType = typeof(NullLog);

		/// <summary>
		/// Sets up logging to be with a certain type
		/// </summary>
		/// <typeparam name="T">The type of ILog for the application to use</typeparam>
		public static void InitializeWith<T>() where T : ILog, new()
		{
			_logType = typeof(T);
		}

		/// <summary>
		/// Initializes a new instance of a logger for an object.
		/// This should be done only once per object name.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>ILog instance for an object if log type has been initialized; otherwise null</returns>
		public static ILog GetLoggerFor(Type objectType)
		{
			var logger = Activator.CreateInstance(_logType) as ILog;
			logger?.InitializeFor(objectType);

			return logger;
		}
	}
}