using System;

namespace Luny
{
	/// <summary>
	/// Engine-agnostic logger entry point. Delegates to an installable engine-specific logger.
	/// Default fallback logs to console until an engine replaces it.
	/// </summary>
	public static class LunyLogger
	{
		private static ILunyLogger _logger = new ConsoleLogger();

		/// <summary>
		/// Installs an engine-specific logger. Pass <c>null</c> to revert to the default console logger.
		/// </summary>
		public static void SetLogger(ILunyLogger logger) => _logger = logger ?? new ConsoleLogger();

		public static void LogInfo(String message) => _logger.LogInfo(message);
		public static void LogWarning(String message) => _logger.LogWarning(message);
		public static void LogError(String message) => _logger.LogError(message);
		public static void LogException(Exception exception) => _logger.LogException(exception);

		private sealed class ConsoleLogger : ILunyLogger
		{
			public void LogInfo(String message) => Console.WriteLine(message);
			public void LogWarning(String message) => Console.WriteLine(message);
			public void LogError(String message) => Console.WriteLine(message);
			public void LogException(Exception exception) => Console.WriteLine(exception?.ToString());
		}
	}

	public interface ILunyLogger
	{
		void LogInfo(String message);
		void LogWarning(String message);
		void LogError(String message);
		void LogException(Exception exception);
	}
}
