using System;

namespace Luny
{
	/// <summary>
	/// LunyScript lifecycle events.
	/// </summary>
	[Flags]
	public enum LunyEngineLifecycleEvents
	{
		/// <summary>
		/// Runs once when application (runtime player) launches. Does not re-run when loading scenes.
		/// </summary>
		OnEngineStartup = 1 << 0,
		/// <summary>
		/// Runs once when application (runtime player) exits.
		/// </summary>
		OnEngineShutdown = 1 << 1,
		/// <summary>
		/// Runs at the beginning of a frame, before update and fixed step.
		/// </summary>
		OnEnginePreUpdate = 1 << 2,
		/// <summary>
		/// Runs at the end of a frame, after late update.
		/// </summary>
		OnEnginePostUpdate = 1 << 3,
		/// <summary>
		/// Runs in sync with engine's "fixed update" or "physics processing" event.
		/// </summary>
		OnEngineHeartbeat = 1 << 4,
		/// <summary>
		/// Runs in sync with engine's "update/process" event.
		/// </summary>
		OnEngineUpdate = 1 << 5,
		/// <summary>
		/// Runs in sync with engine's "late update" event.
		/// Where unsupported (Godot) the native engine adapter calls this at the end of the engine's update event.
		/// </summary>
		OnEngineLateUpdate = 1 << 6,

		OnEngineSceneUnloaded = 1 << 8,
		OnEngineSceneLoaded = 1 << 9,

		OnObjectCreated = 1 << 10,
		OnObjectDestroyed = 1 << 11,
	}
}
