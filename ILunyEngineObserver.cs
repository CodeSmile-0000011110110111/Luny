using Luny.Engine.Bridge;
using System;

namespace Luny
{
	/// <summary>
	/// Lifecycle observer interface. To be implemented by observers of LunyEngine which wish to receive the engine-agnostic lifecycle callbacks.
	/// </summary>
	public interface ILunyEngineObserver
	{
		public Boolean Enabled => true;

		/// <summary>
		/// Runs when the application launches (once-only). Processing of the first frame has not begun.
		/// </summary>
		void OnEngineStartup();

		/// <summary>
		/// Runs once per frame, before any frame update / heartbeat method.
		/// </summary>
		void OnEngineFrameBegins() {}

		/// <summary>
		/// Runs once per frame, after all frame update methods ran (eg "end of frame").
		/// </summary>
		void OnEngineFrameEnds() {}

		/// <summary>
		/// Runs on the engine's fixed stepping frequency. Most suitable for deterministic game logic and to modify the Physics simulation.
		/// </summary>
		/// <remarks>
		/// Engine internal physics simulation occurs right after Heartbeat, before FrameUpdate.
		///
		/// Caution: Heartbeat frequency depends on engine time-stepping settings, and is not guaranteed to be in sync with FrameRate.
		/// The behaviour depends on each frame's delta time (frame rate) and the engine's fixed step (or physics) time setting.
		///		- Heartbeat may be called less often than FrameUpdate
		///		- Heartbeat may be called several times in a single frame (multiple times before FrameUpdate)
		/// </remarks>
		void OnEngineHeartbeat();

		/// <summary>
		/// Runs once per frame. It runs right after the Heartbeat (if any).
		/// </summary>
		void OnEngineFrameUpdate();

		/// <summary>
		/// Runs once per frame, after all FrameUpdate ran.
		/// </summary>
		void OnEngineFrameLateUpdate() {}

		/// <summary>
		/// Runs when the application exits (once-only). Runs after frame processing has completed.
		/// </summary>
		void OnEngineShutdown();

		/// <summary>
		/// Runs when a scene was loaded, before frame processing begins. Also fires in the first scene the engine launches with.
		/// </summary>
		/// <param name="loadedScene"></param>
		void OnSceneLoaded(ILunyScene loadedScene) {}

		/// <summary>
		/// Runs when a scene was unloaded. After all previous scene's objects have been invalidated (destroyed).
		/// </summary>
		/// <param name="unloadedScene"></param>
		void OnSceneUnloaded(ILunyScene unloadedScene) {}

		/// <summary>
		/// Called when a LunyObject registered with LunyEngine either via instantiation or handing ownership of an existing
		/// engine object to LunyEngine. Runs before the object's OnCreated callback.
		/// </summary>
		/// <param name="lunyGameObject"></param>
		void OnObjectRegistered(ILunyGameObject lunyGameObject) {}

		/// <summary>
		/// Called when a LunyObject has unregistered from LunyEngine either by destroying it or when transferring ownership back
		/// to the native engine. Runs after object's OnDestroy callback. The object's `IsValid` is false but its NativeObject reference
		/// can still be accessed.
		/// </summary>
		/// <param name="lunyGameObject"></param>
		void OnObjectUnregistered(ILunyGameObject lunyGameObject) {}
	}
}
