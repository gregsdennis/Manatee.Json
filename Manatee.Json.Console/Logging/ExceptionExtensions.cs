using System;
using System.Text;

namespace Manatee.Json.Console.Logging
{
	public static class ExceptionExtensions
	{
		public static string ExtendedMessage(this Exception ex)
		{
			var sb = new StringBuilder();
			while (ex != null)
			{
				sb.AppendLine(ex.Message);
				sb.AppendLine(ex.StackTrace);
				ex = ex.InnerException;
			}

			return sb.ToString();
		}
	}
}