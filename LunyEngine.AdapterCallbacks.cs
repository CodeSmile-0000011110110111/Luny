using Luny.Engine.Bridge;
using Luny.Engine.Services;
using System;

namespace Luny
{
	public sealed partial class LunyEngine
	{
		private Boolean _didCallPreUpdateThisFrame;
		private Boolean _engineStartupCompleted;

		void ILunyEngineLifecycle.EngineStartup(ILunyEngineNativeAdapter nativeAdapter)
		{
			_timeInternal.SetLunyFrameCount(1); // Startup is enforced to be in frame 1

			ILunyEngineLifecycle.ThrowIfNotCurrentAdapter(nativeAdapter, s_EngineAdapter);
			LunyTraceLogger.LogInfoStartingUp(this);

			// Observers Startup
			foreach (var observer in _observerRegistry.EnabledObservers)
			{
				_profiler.BeginObserver(observer);
				try
				{
					observer.OnEngineStartup();
				}
				catch (Exception e)
				{
					_profiler.RecordError(observer, LunyEngineLifecycleEvents.OnEngineStartup, e);
					//LunyLogger.LogException(e, this);
					throw;
				}
				finally
				{
					_profiler.EndObserver(observer, LunyEngineLifecycleEvents.OnEngineStartup);
				}
			}

			_serviceRegistry.Startup();
			_engineStartupCompleted = true;

			// process any "too early" scene events
			if (_sceneLoadEventQueue != null)
			{
				foreach (var lunyScene in _sceneLoadEventQueue)
					OnSceneLoaded(lunyScene);

				_sceneLoadEventQueue = null;
			}

			LunyTraceLogger.LogInfoStartupComplete(this);
			_timeInternal.SetLunyFrameCount(0); // Reset back to 0 since we increment it when processing the first frame
		}

		void ILunyEngineLifecycle.EngineShutdown(ILunyEngineNativeAdapter nativeAdapter)
		{
			LunyTraceLogger.LogInfoShuttingDown(this);
			ILunyEngineLifecycle.ThrowIfNotCurrentAdapter(nativeAdapter, s_EngineAdapter);

			// Observers Shutdown
			foreach (var observer in _observerRegistry.EnabledObservers)
			{
				_profiler.BeginObserver(observer);
				try
				{
					observer.OnEngineShutdown();
				}
				catch (Exception e)
				{
					_profiler.RecordError(observer, LunyEngineLifecycleEvents.OnEngineShutdown, e);
					//LunyLogger.LogException(e, this);
					throw;
				}
				finally
				{
					_profiler.EndObserver(observer, LunyEngineLifecycleEvents.OnEngineShutdown);
				}
			}

			// Services & Engine Shutdown
			try
			{
				var sceneService = (ILunySceneServiceInternal)Scene;
				sceneService.OnSceneLoaded -= OnSceneLoaded;
				sceneService.OnSceneUnloaded -= OnSceneUnloaded;

				_profiler.Shutdown();
				_objectLifecycle.Shutdown(_objectRegistry);
				_objectRegistry.Shutdown();
				_serviceRegistry.Shutdown();
				_observerRegistry.Shutdown();
			}
			catch (Exception)
			{
				LunyLogger.LogError($"Error during {nameof(LunyEngine)} {nameof(ILunyEngineLifecycle.EngineShutdown)}!", this);
				throw;
			}
			finally
			{
				_serviceRegistry = null;
				_observerRegistry = null;
				_objectRegistry = null;
				_objectLifecycle = null;
				_profiler = null;
				_timeInternal = null;
				LunyPath.Converter = null;

				// ensure we won't get re-instantiated after this point
				s_IsDisposed = true;
				s_EngineAdapter = null;
				s_Instance = null;
				GC.SuppressFinalize(this);

				LunyTraceLogger.LogInfoShutdownComplete(this);
			}
		}

		void ILunyEngineLifecycle.EngineHeartbeat(ILunyEngineNativeAdapter nativeAdapter, Double fixedDeltaTime)
		{
			ILunyEngineLifecycle.ThrowIfNotCurrentAdapter(nativeAdapter, s_EngineAdapter);

			_timeInternal.SetFixedDeltaTime(fixedDeltaTime);
			RunEngineFrameBegins();

			foreach (var observer in _observerRegistry.EnabledObservers)
			{
				_profiler.BeginObserver(observer);

				_serviceRegistry.OnEngineHeartbeat();

				try
				{
					observer.OnEngineHeartbeat();
				}
				catch (Exception e)
				{
					_profiler.RecordError(observer, LunyEngineLifecycleEvents.OnEngineHeartbeat, e);
					/* keep dispatch resilient */
					//LunyLogger.LogException(e, this);
					throw;
				}
				finally
				{
					_profiler.EndObserver(observer, LunyEngineLifecycleEvents.OnEngineHeartbeat);
				}
			}
		}

		void ILunyEngineLifecycle.EngineFrameUpdate(ILunyEngineNativeAdapter nativeAdapter, Double deltaTime)
		{
			ILunyEngineLifecycle.ThrowIfNotCurrentAdapter(nativeAdapter, s_EngineAdapter);

			_timeInternal.SetDeltaTime(deltaTime);
			RunEngineFrameBegins();

			foreach (var observer in _observerRegistry.EnabledObservers)
			{
				_profiler.BeginObserver(observer);

				_serviceRegistry.OnEngineFrameUpdate();

				try
				{
					observer.OnEngineFrameUpdate();
				}
				catch (Exception e)
				{
					_profiler.RecordError(observer, LunyEngineLifecycleEvents.OnEngineUpdate, e);
					/* keep dispatch resilient */
					//LunyLogger.LogException(e, this);
					throw;
				}
				finally
				{
					_profiler.EndObserver(observer, LunyEngineLifecycleEvents.OnEngineUpdate);
				}
			}
		}

		void ILunyEngineLifecycle.EngineFrameLateUpdate(ILunyEngineNativeAdapter nativeAdapter)
		{
			ILunyEngineLifecycle.ThrowIfNotCurrentAdapter(nativeAdapter, s_EngineAdapter);

			foreach (var observer in _observerRegistry.EnabledObservers)
			{
				_profiler.BeginObserver(observer);

				_serviceRegistry.OnEngineFrameLateUpdate();

				try
				{
					observer.OnEngineFrameLateUpdate();
				}
				catch (Exception e)
				{
					_profiler.RecordError(observer, LunyEngineLifecycleEvents.OnEngineLateUpdate, e);
					/* keep dispatch resilient */
					//LunyLogger.LogException(e, this);
					throw;
				}
				finally
				{
					_profiler.EndObserver(observer, LunyEngineLifecycleEvents.OnEngineLateUpdate);
				}
			}

			RunEngineFrameEnds();
		}

		private void RunEngineFrameBegins()
		{
			if (!_didCallPreUpdateThisFrame)
			{
				_didCallPreUpdateThisFrame = true;

				// engine services first
				_timeInternal.IncrementFrameCount();
				_serviceRegistry.OnEngineFrameBegins();
				_objectLifecycle.OnEngineFrameBegins();

				foreach (var observer in _observerRegistry.EnabledObservers)
				{
					_profiler.BeginObserver(observer);
					try
					{
						observer.OnEngineFrameBegins();
					}
					catch (Exception e)
					{
						_profiler.RecordError(observer, LunyEngineLifecycleEvents.OnEnginePreUpdate, e);
						/* keep dispatch resilient */
						//LunyLogger.LogException(e, this);
						throw;
					}
					finally
					{
						_profiler.EndObserver(observer, LunyEngineLifecycleEvents.OnEnginePreUpdate);
					}
				}
			}
		}

		private void RunEngineFrameEnds()
		{
			foreach (var observer in _observerRegistry.EnabledObservers)
			{
				_profiler.BeginObserver(observer);
				try
				{
					observer.OnEngineFrameEnds();
				}
				catch (Exception e)
				{
					_profiler.RecordError(observer, LunyEngineLifecycleEvents.OnEnginePostUpdate, e);
					/* keep dispatch resilient */
					//LunyLogger.LogException(e, this);
					throw;
				}
				finally
				{
					_profiler.EndObserver(observer, LunyEngineLifecycleEvents.OnEnginePostUpdate);
				}
			}

			// run "structural changes" here ..
			_serviceRegistry.OnEngineFrameEnds();
			_objectLifecycle.OnEngineFrameEnds(); // should run last to guarantee object cleanup

			_didCallPreUpdateThisFrame = false;
		}
	}
}
