using Luny.Engine.Bridge;
using Luny.Engine.Diagnostics;
using Luny.Engine.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace Luny
{
	/// <summary>
	/// See implementation: <see cref="Luny.LunyEngine"/>
	/// </summary>
	public interface ILunyEngine
	{
		/// <summary>
		/// Application-level services, similar to [UnityEngine.Application](xref:UnityEngine.Application) and
		/// [UnityEditor.EditorApplication](xref:UnityEditor.EditorApplication).
		/// </summary>
		ILunyApplicationService Application { get; }
		/// <summary>
		/// Asset loading/unloading and caching. Enables asset lookup by path/name. Returns placeholder assets instead of throwing exceptions.
		/// </summary>
		ILunyAssetService Asset { get; }
		/// <summary>
		/// Debug services, eg logging.
		/// </summary>
		ILunyDebugService Debug { get; }
		/// <summary>
		/// Editor services. Safe to call in runtime code (no-op, returning defaults).
		/// </summary>
		ILunyEditorService Editor { get; }
		/// <summary>
		/// Input services, forwards Action Map events and provides access to most recent input state.
		/// </summary>
		ILunyInputService Input { get; }
		/// <summary>
		/// Object services, mainly creating new prefab/primitive instances.
		/// </summary>
		ILunyObjectService Object { get; }
		/// <summary>
		/// Scene services, including in-scene object queries by name/path/type and scene load/unload.
		/// </summary>
		ILunySceneService Scene { get; }
		/// <summary>
		/// Time services, eg delta time, elapsed seconds, frame count.
		/// </summary>
		ILunyTimeService Time { get; }

		/// <summary>
		/// In-scene LunyObject instances get registered (cached) here after creation or ownership transfer to LunyEngine.
		/// </summary>
		ILunyObjectRegistry Objects { get; }
		/// <summary>
		/// LunyEngine profiler maintains a runtime record of observer execution metrics.
		/// </summary>
		ILunyEngineProfiler Profiler { get; }

		/// <summary>
		/// Will try to find an object by name in the scene. Queries already-cached objects first. Wraps found objects in a `LunyObject` instance.
		/// </summary>
		LunyGameObject TryGetObject(String name, [CallerFilePath] String callerFilePath = "", [CallerLineNumber] Int32 callerLineNumber = 0);

		// Observer management
		void EnableObserver<T>() where T : ILunyEngineObserver;
		void DisableObserver<T>() where T : ILunyEngineObserver;
		Boolean IsObserverEnabled<T>() where T : ILunyEngineObserver;
		T GetObserver<T>() where T : ILunyEngineObserver;

		/// <summary>
		/// Gets a service by type. Throws if the service is not registered. Note: essential services are exposed as properties.
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		TService GetService<TService>() where TService : LunyEngineServiceBase;

		/// <summary>
		/// Gets a service by type, may return false/null.
		/// </summary>
		/// <param name="service"></param>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		Boolean TryGetService<TService>(out TService service) where TService : LunyEngineServiceBase;

		/// <summary>
		/// Queries if a service type is registered.
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		Boolean HasService<TService>() where TService : LunyEngineServiceBase;
	}

	internal interface ILunyEngineInternal
	{
		ILunyObjectLifecycleInternal ObjectLifecycle { get; }
	}

	/// <summary>
	/// LunyEngine singleton discovers and manages engine services and observers.
	/// </summary>
	public sealed partial class LunyEngine : ILunyEngine, ILunyEngineInternal, ILunyEngineLifecycle
	{
		private static LunyEngine s_Instance;
		private static ILunyEngineNativeAdapter s_EngineAdapter;
		private static Boolean s_IsDisposed;

		private LunyServiceRegistry _serviceRegistry;
		private LunyEngineObserverRegistry _observerRegistry;
		private LunyObjectRegistry _objectRegistry;
		private LunyObjectLifecycle _objectLifecycle;
		private LunyEngineProfiler _profiler;
		private ILunyTimeServiceInternal _timeInternal;

		// API Services
		public ILunyApplicationService Application { get; private set; }
		public ILunyAssetService Asset { get; private set; }
		public ILunyDebugService Debug { get; private set; }
		public ILunyEditorService Editor { get; private set; }
		public ILunyInputService Input { get; private set; }
		public ILunyObjectService Object { get; private set; }
		public ILunySceneService Scene { get; private set; }
		public ILunyTimeService Time { get; private set; }

		ILunyObjectRegistry ILunyEngine.Objects => _objectRegistry;
		ILunyObjectLifecycleInternal ILunyEngineInternal.ObjectLifecycle => _objectLifecycle;

		/// <summary>
		/// Gets the engine profiler for performance monitoring.
		/// Profiling methods are no-ops in release builds unless LUNY_PROFILE is defined.
		/// </summary>
		public ILunyEngineProfiler Profiler => _profiler;

		/// <summary>
		/// Gets the singleton instance, creating it on first access.
		/// </summary>
		public static ILunyEngine Instance => s_Instance;

		[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
		internal static ILunyEngineLifecycle CreateInstance(ILunyEngineNativeAdapter engineAdapter)
		{
			LunyTraceLogger.LogInfoCreateSingletonInstance(typeof(LunyEngine));
			if (s_IsDisposed)
				throw new LunyLifecycleException($"{nameof(LunyEngine)} instance already disposed. It must not be created again.");
			if (s_Instance != null)
				throw new LunyLifecycleException($"{nameof(LunyEngine)} instance already exists.");
			if (engineAdapter == null) // adapter instance is used to ensure only the creating adapter can run LunyEngine
				throw new ArgumentNullException(nameof(engineAdapter), $"{nameof(ILunyEngineNativeAdapter)} cannot be null");

			s_EngineAdapter = engineAdapter;

			// splitting ctor and Initialize prevents stackoverflows for cases where Instance is accessed from within ctor
			s_Instance = new LunyEngine();
			s_Instance.Initialize(engineAdapter.Engine);
			return s_Instance;
		}

		internal static void ForceReset_UnityEditorAndUnitTestsOnly()
		{
			s_IsDisposed = false;
			ILunyEngineNativeAdapter.IsApplicationQuitting = false;
		}

		private LunyEngine() => ILunyEngineLifecycle.ThrowOnSingletonDuplication(s_Instance);

		private void Initialize(NativeEngine engine)
		{
			try
			{
				LunyTraceLogger.LogInfoInitializing(this);

				LunyObjectId.Reset();
				LunyAssetId.Reset();

				_serviceRegistry = new LunyServiceRegistry(engine);
				AssignMandatoryServices();

				_timeInternal = (ILunyTimeServiceInternal)Time;
				_timeInternal.SetLunyFrameCount(0); // frame "0" marks anything before OnEngineStartup()

				var sceneService = (ILunySceneServiceInternal)Scene;
				sceneService.OnSceneLoaded += OnSceneLoaded;
				sceneService.OnSceneUnloaded += OnSceneUnloaded;

				_profiler = new LunyEngineProfiler(Time);
				_observerRegistry = new LunyEngineObserverRegistry();
				_objectRegistry = new LunyObjectRegistry();
				_objectLifecycle = new LunyObjectLifecycle();

				LunyTraceLogger.LogInfoInitialized(this);
			}
			catch (Exception)
			{
				LunyLogger.LogError($"Error during {nameof(LunyEngine)} {nameof(Initialize)}!", this);
				throw;
			}
		}

		public Boolean HasService<TService>() where TService : LunyEngineServiceBase => _serviceRegistry.Has<TService>();
		public TService GetService<TService>() where TService : LunyEngineServiceBase => _serviceRegistry.Get<TService>();

		public Boolean TryGetService<TService>(out TService service) where TService : LunyEngineServiceBase =>
			_serviceRegistry.TryGet(out service);

		public void EnableObserver<T>() where T : ILunyEngineObserver => _observerRegistry.EnableObserver<T>();
		public void DisableObserver<T>() where T : ILunyEngineObserver => _observerRegistry.DisableObserver<T>();
		public Boolean IsObserverEnabled<T>() where T : ILunyEngineObserver => _observerRegistry.IsObserverEnabled<T>();
		public T GetObserver<T>() where T : ILunyEngineObserver => _observerRegistry.GetObserver<T>();

		public LunyGameObject TryGetObject(String name, [CallerFilePath] String callerFilePath = "", [CallerLineNumber] Int32 callerLineNumber = 0)
		{
			var obj = _objectRegistry.GetCached(name) ?? _objectRegistry.Find(name);
			if (obj == null)
			{
				LunyLogger.LogWarning($"Object '{name}' was not found in scene. " +
				                      $"({Path.GetFileName(callerFilePath)}({callerLineNumber}))", this);
			}

			return obj;
		}

		private void AssignMandatoryServices()
		{
			Application = GetService<LunyApplicationServiceBase>();
			Asset = GetService<LunyAssetServiceBase>();
			Debug = GetService<LunyDebugServiceBase>();
			Editor = GetService<LunyEditorServiceBase>();
			Input = GetService<LunyInputServiceBase>();
			Object = GetService<LunyObjectServiceBase>();
			Scene = GetService<LunySceneServiceBase>();
			Time = GetService<LunyTimeServiceBase>();
		}

		~LunyEngine() => LunyTraceLogger.LogInfoFinalized(this);
	}
}
