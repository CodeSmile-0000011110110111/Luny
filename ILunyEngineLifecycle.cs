using System;

namespace Luny
{
	internal interface ILunyEngineLifecycle
	{
		static void ThrowOnSingletonDuplication(LunyEngine instance)
		{
			if (instance != null)
				throw new LunyLifecycleException($"Duplicate {nameof(LunyEngine)} singleton detected!");
		}

		static void ThrowIfNotCurrentAdapter(ILunyEngineNativeAdapter actualAdapter, ILunyEngineNativeAdapter expectedAdapter)
		{
#if DEBUG || LUNY_DEBUG
			if (actualAdapter == null)
				throw new LunyLifecycleException($"Null adapter passed into {nameof(ILunyEngineLifecycle)} interface method!");
			if (actualAdapter != expectedAdapter)
				throw new LunyLifecycleException($"Wrong adapter {actualAdapter} passed into {nameof(ILunyEngineLifecycle)} interface method!");
#endif
		}

		// Lifecycle callbacks for engine adapter
		void EngineStartup(ILunyEngineNativeAdapter nativeAdapter);
		void EngineHeartbeat(ILunyEngineNativeAdapter nativeAdapter, Double fixedDeltaTime);
		void EngineFrameUpdate(ILunyEngineNativeAdapter nativeAdapter, Double deltaTime);
		void EngineFrameLateUpdate(ILunyEngineNativeAdapter nativeAdapter);
		void EngineShutdown(ILunyEngineNativeAdapter nativeAdapter);
	}
}
