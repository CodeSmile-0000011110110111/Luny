using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace Luny
{
	public interface IEngineReferences
	{
		ILunyGameObject GameObject(String name);
		ILunyComponent Component(String name);
		T Get<T>(String name) where T : struct;
	}

	public sealed class EngineReferences : IEngineReferences
	{
		private readonly Dictionary<String, EngineReference> _references = new();
		private readonly Func<Object, ILunyGameObject> _gameObjectFactory;
		private readonly Func<Object, ILunyComponent> _componentFactory;

		public EngineReferences(Func<Object, ILunyGameObject> gameObjectFactory, Func<Object, ILunyComponent> componentFactory)
		{
			_gameObjectFactory = gameObjectFactory ?? throw new ArgumentNullException(nameof(gameObjectFactory));
			_componentFactory = componentFactory ?? throw new ArgumentNullException(nameof(componentFactory));
		}

		internal void Add(String key, Object value, EngineReferenceType type) => _references[key] = new EngineReference
		{
			Name = key,
			Value = value,
			Type = type,
		};

		public ILunyGameObject GameObject(String name)
		{
			if (!_references.TryGetValue(name, out var r))
				return null;
			return _gameObjectFactory(r.Value);
		}

		public ILunyComponent Component(String name)
		{
			if (!_references.TryGetValue(name, out var r))
				return null;
			return _componentFactory(r.Value);
		}

		public T Get<T>(String name) where T : struct
		{
			if (!_references.TryGetValue(name, out var r) || r.Value is not T value)
				return default;
			return value;
		}
	}
}
