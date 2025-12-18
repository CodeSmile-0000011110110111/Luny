using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Luny
{
	/// <summary>
	/// Generic service registry that discovers and holds engine provider services.
	/// </summary>
	/// <typeparam name="T">Service interface type that must implement IEngineProvider</typeparam>
	public sealed class EngineServiceRegistry<T> where T : class, IEngineProvider
	{
		private readonly Dictionary<Type, T> _registeredServices = new();

		public IEnumerable<T> Services => _registeredServices.Values;

		public EngineServiceRegistry() => DiscoverAndInstantiateServices();

		private void DiscoverAndInstantiateServices()
		{
			LunyLogger.LogInfo($"Discovering {typeof(T).Name} services...", this);
			var sw = Stopwatch.StartNew();

			var serviceTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a =>
				{
					try { return a.GetTypes(); }
					catch { return Array.Empty<Type>(); }
				})
				.Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

			foreach (var type in serviceTypes)
			{
				LunyLogger.LogInfo($"Registering service: {type.Name} (Assembly: {type.Assembly.GetName().Name})", this);
				var service = (T)Activator.CreateInstance(type);
				_registeredServices[type] = service;
			}

			sw.Stop();

			var ms = (Int32)Math.Round(sw.Elapsed.TotalMilliseconds, MidpointRounding.AwayFromZero);
			LunyLogger.LogInfo($"Registered {_registeredServices.Count} {typeof(T).Name} services in {ms} ms.", this);
		}

		public TService Get<TService>() where TService : class, T, IEngineProvider
		{
			if (_registeredServices.TryGetValue(typeof(TService), out var service))
				return service as TService;

			Throw.ServiceNotFoundException(typeof(TService).Name);
			return null;
		}

		public Boolean Has<TService>() where TService : class, T, IEngineProvider =>
			_registeredServices.ContainsKey(typeof(TService));
	}
}
