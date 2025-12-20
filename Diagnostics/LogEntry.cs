using System;

namespace Luny.Diagnostics
{
	/// <summary>
	/// Represents a single log entry in the internal Luny logging system.
	/// Uses FrameCount and ElapsedSeconds instead of DateTime for precise game-time tracking.
	/// </summary>
	public struct LogEntry
	{
		/// <summary>
		/// Frame number when this log entry was created (from ITimeServiceProvider).
		/// -1 if time service not available.
		/// </summary>
		public Int64 FrameCount;

		/// <summary>
		/// Elapsed time in seconds since application start (from ITimeServiceProvider).
		/// -1.0 if time service not available.
		/// </summary>
		public Double ElapsedSeconds;

		/// <summary>
		/// Severity level of the log message.
		/// </summary>
		public LogLevel Level;

		/// <summary>
		/// The log message content.
		/// </summary>
		public String Message;

		/// <summary>
		/// Context type name that generated this log (e.g., "LunyScriptRunner", "LogMessageBlock").
		/// </summary>
		public String Context;

		public override String ToString() =>
			$"[F{FrameCount:D8}] [{ElapsedSeconds:F3}s] [{Level}] [{Context}] {Message}";
	}

	/// <summary>
	/// Log message severity levels.
	/// </summary>
	public enum LogLevel
	{
		Info,
		Warning,
		Error
	}
}
