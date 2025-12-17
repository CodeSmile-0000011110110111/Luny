using System;

namespace Luny
{
	/// <summary>
	/// Lifecycle observer interface - receives callbacks from dispatcher.
	/// </summary>
	public interface IEngineLifecycle
	{
		// Lifecycle observer interface - receives callbacks from dispatcher
		void OnStartup();
		void OnFixedStep(Double fixedDeltaTime);
		void OnUpdate(Double deltaTime);
		void OnLateUpdate(Double deltaTime);
		void OnShutdown();
	}
}
