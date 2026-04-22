using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace Luny
{
	public interface IEngineReferences
	{
		LunyGameObject this[String name] { get; }
		LunyGameObject GetGameObject(String name);
		LunyComponent GetComponent(String name);
		T Get<T>(String name);
	}

	public sealed class EngineReferences : IEngineReferences
	{
		private readonly Dictionary<String, EngineReference> _references = new();
		private readonly Func<Object, LunyGameObject> _gameObjectFactory;
		private readonly Func<Object, LunyComponent> _componentFactory;

		public LunyGameObject this[String name] => GetGameObject(name);

		public EngineReferences(Func<Object, LunyGameObject> gameObjectFactory, Func<Object, LunyComponent> componentFactory)
		{
			_gameObjectFactory = gameObjectFactory ?? throw new ArgumentNullException(nameof(gameObjectFactory));
			_componentFactory = componentFactory ?? throw new ArgumentNullException(nameof(componentFactory));
		}

		public LunyGameObject GetGameObject(String name)
		{
			if (!_references.TryGetValue(name, out var r))
				return null;

			return _gameObjectFactory(r.Value);
		}

		public LunyComponent GetComponent(String name)
		{
			if (!_references.TryGetValue(name, out var r))
				return null;

			return _componentFactory(r.Value);
		}

		public T Get<T>(String name)
		{
			if (!_references.TryGetValue(name, out var r) || r.Value is not T value)
				return default;

			return value;
		}

		internal void Add(String key, Object value, EngineReferenceType type) => _references[key] = new EngineReference
		{
			Name = key,
			Value = value,
			Type = type,
		};
	}
}
