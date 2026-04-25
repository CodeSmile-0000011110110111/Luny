using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace Luny
{
	public interface IEngineReferences
	{
		Object this[String name] { get; }
		LunyGameObject GetGameObject(String name);

		LunyTransform GetTransform(String name);

		//LunyComponent GetComponent(String name);
		T Get<T>(String name);
	}

	public sealed class EngineReferences : IEngineReferences
	{
		private readonly Dictionary<String, EngineReference> _references = new();
		private readonly Dictionary<Type, Func<Object, Object>> _factories = new();

		public Object this[String name] => Get<Object>(name);

		public EngineReferences(IEnumerable<KeyValuePair<Type, Func<Object, Object>>> factories)
		{
			if (factories != null)
			{
				foreach (var pair in factories)
					_factories[pair.Key] = pair.Value;
			}
		}

		public LunyGameObject GetGameObject(String name)
		{
			if (!TryGetReference(name, out var r))
				return null;

			if (TryGetFactory<LunyGameObject>(out var factory))
				return factory(r.Value) as LunyGameObject;

			return default;
		}

		public LunyTransform GetTransform(String name)
		{
			if (!TryGetReference(name, out var r))
				return null;

			if (TryGetFactory<LunyTransform>(out var factory))
				return factory(r.Value) as LunyTransform;

			return default;
		}

		/*public LunyComponent GetComponent(String name)
		{
			if (!_references.TryGetValue(name, out var r))
				return null;

			if (_factories.TryGetValue(typeof(LunyComponent), out var factory))
				return (LunyComponent)factory(r.Value);

			return default;
		}*/

		public T Get<T>(String name) => TryGetReference(name, out var r) && r.Value is T value ? value : default;
		private Boolean TryGetFactory<T>(out Func<Object, Object> factory) => _factories.TryGetValue(typeof(T), out factory);
		private Boolean TryGetReference(String name, out EngineReference r) => _references.TryGetValue(name, out r);

		internal void Add(String key, Object value, EngineReferenceType type) => _references[key] = new EngineReference
		{
			Name = key,
			Value = value,
			Type = type,
		};
	}

	public record EngineReference
	{
		public String Name;
		public Object Value;
		public EngineReferenceType Type;
	}

	public enum EngineReferenceType
	{
		// Base object types
		Object = 0,
		GameObject = 1,

		// Specific component types
		// Component = 5000,
		// Transform = 5001,
		// Rigidbody = 5002,

		// Asset Types
		ScriptableObject = 10000,
		Material = 10001,
		// AudioClip = 10002,
		// Mesh = 10003,

		// Value types (non-Object)
		Color = 20000,
		AnimationCurve = 20001,
		Vector2 = 20100,
		Vector3 = 20101,
	}
}
