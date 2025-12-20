using System;
using System.Collections.Generic;

namespace Luny.Diagnostics
{
	/// <summary>
	/// Immutable snapshot of profiler state at a specific point in time.
	/// Useful for querying performance metrics without blocking the profiler.
	/// </summary>
	public sealed class ProfilerSnapshot
	{
		public IReadOnlyList<ObserverMetrics> ObserverMetrics;
		public DateTime Timestamp;

		public override String ToString() =>
			$"ProfilerSnapshot @ {Timestamp:HH:mm:ss.fff}: {ObserverMetrics.Count} observers";
	}
}
