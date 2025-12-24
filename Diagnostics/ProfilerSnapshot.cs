using System;
using System.Collections.Generic;

namespace Luny.Diagnostics
{
	public interface IProfilerSnapshot
	{
		IReadOnlyList<ObserverMetrics> ObserverMetrics { get; }
		DateTime Timestamp { get; }
		Int64 FrameCount { get; }
	}

	/// <summary>
	/// Immutable snapshot of profiler state at a specific point in time.
	/// Useful for querying performance metrics without blocking the profiler.
	/// </summary>
	public sealed class ProfilerSnapshot : IProfilerSnapshot
	{
		public IReadOnlyList<ObserverMetrics> ObserverMetrics { get; internal set; }
		public DateTime Timestamp { get; internal set; }
		public Int64 FrameCount { get; internal set; }

		public override String ToString() => $"ProfilerSnapshot @ {Timestamp:HH:mm:ss.fff}: {ObserverMetrics.Count} observers";
	}
}
