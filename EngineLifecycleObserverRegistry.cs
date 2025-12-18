using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Luny
{
	/// <summary>
	/// Registry that discovers, manages, and enables/disables lifecycle observers.
	/// </summary>
	internal sealed class EngineLifecycleObserverRegistry
	{
		private readonly Dictionary<Type, IEngineLifecycleObserver> _registeredObservers = new();
		private readonly List<IEngineLifecycleObserver> _enabledObservers = new();

		public IEnumerable<IEngineLifecycleObserver> EnabledObservers => _enabledObservers;

		public EngineLifecycleObserverRegistry() => DiscoverAndInstantiateObservers();

		private void DiscoverAndInstantiateObservers()
		{
			LunyLogger.LogInfo($"Locating {nameof(IEngineLifecycleObserver)} observers ...", this);
			var sw = Stopwatch.StartNew();

			var observerTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a =>
				{
					try { return a.GetTypes(); }
					catch { return Array.Empty<Type>(); }
				})
				.Where(t => typeof(IEngineLifecycleObserver).IsAssignableFrom(t) && !t.IsAbstract);

			// TODO: sort observers deterministically
			// TODO: configure observer enabled states
			// TODO: filter [LunyTestable] observers unless in test scenario

			foreach (var type in observerTypes)
			{
				LunyLogger.LogInfo($"Creating observer instance: {type.Name} (Assembly: {type.Assembly.GetName().Name})", this);
				var observer = (IEngineLifecycleObserver)Activator.CreateInstance(type);
				_registeredObservers[type] = observer;
				_enabledObservers.Add(observer); // enabled by default
			}

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Registered {_enabledObservers.Count} {nameof(IEngineLifecycleObserver)} observers in {ms} ms.", this);
		}

		public void EnableObserver<T>() where T : IEngineLifecycleObserver
		{
			if (_registeredObservers.TryGetValue(typeof(T), out var observer))
			{
				if (!_enabledObservers.Contains(observer))
					_enabledObservers.Add(observer);
			}
		}

		public void DisableObserver<T>() where T : IEngineLifecycleObserver
		{
			if (_registeredObservers.TryGetValue(typeof(T), out var observer))
				_enabledObservers.Remove(observer);
		}

		public Boolean IsObserverEnabled<T>() where T : IEngineLifecycleObserver =>
			_registeredObservers.TryGetValue(typeof(T), out var observer) &&
			_enabledObservers.Contains(observer);
	}
}
