namespace Luny.Engine.Services
{
	/// <summary>
	/// Marker interface for engine-agnostic services: APIs such as Debug, Input, etc.
	/// Implementations are auto-discovered and registered at startup.
	/// </summary>
	public interface ILunyEngineService {}

	public abstract class LunyEngineServiceBase : ILunyEngineService
	{
		internal void Initialize()
		{
			LunyTraceLogger.LogInfoInitializing(this);
			OnServiceInitialize();
			LunyTraceLogger.LogInfoInitialized(this);
		}

		internal void Startup()
		{
			LunyTraceLogger.LogInfoStartingUp(this);
			OnServiceStartup();
			LunyTraceLogger.LogInfoStartupComplete(this);
		}

		internal void Shutdown()
		{
			LunyTraceLogger.LogInfoShuttingDown(this);
			OnServiceShutdown();
			LunyTraceLogger.LogInfoShutdownComplete(this);
		}

		internal void FrameBegins() => OnServiceFrameBegins();
		internal void Heartbeat() => OnServiceHeartbeat();
		internal void FrameUpdate() => OnServiceFrameUpdate();
		internal void FrameLateUpdate() => OnServiceFrameLateUpdate();
		internal void FrameEnds() => OnServiceFrameEnds();
		protected virtual void OnServiceInitialize() {}
		protected virtual void OnServiceStartup() {}
		protected virtual void OnServiceShutdown() {}
		protected virtual void OnServiceFrameBegins() {}
		protected virtual void OnServiceHeartbeat() {}
		protected virtual void OnServiceFrameUpdate() {}
		protected virtual void OnServiceFrameLateUpdate() {}
		protected virtual void OnServiceFrameEnds() {}
	}
}
