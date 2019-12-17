namespace Manatee.Json
{
	/// <summary>
	/// Enables verbose logging during serialization and schema processing.
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Creates a log entry at a "verbose" log level.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="category">The logging category.</param>
		void Verbose(string message, LogCategory category = LogCategory.General);
	}
}