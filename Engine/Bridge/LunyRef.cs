using System;
using System.Diagnostics.CodeAnalysis;

namespace Luny.Engine.Bridge
{
	public abstract class LunyRef<T> where T : class
	{
		private String _query;
		protected WeakReference<T> _cachedObject;

		public T Value
		{
			get
			{
				if (!IsCachedObjectValid(out var obj))
				{
					obj = ResolveSceneObject(_query);
					//LunyLogger.LogInfo($"'{_query}' resolved to {obj}", this);
					_cachedObject.SetTarget(obj);
				}

				return obj;
			}
		}

		protected LunyRef(String query)
		{
			if (String.IsNullOrEmpty(query))
				throw new ArgumentException("Query string cannot be null or empty.", nameof(query));

			_query = query;
			_cachedObject = new WeakReference<T>(default);
		}

		protected LunyRef(T obj)
		{
			if (obj == null)
				throw new ArgumentNullException("Object cannot be null.", nameof(obj));

			_cachedObject = new WeakReference<T>(obj);
		}

		protected Boolean IsCachedObjectValid(out T obj) => _cachedObject.TryGetTarget(out obj) && IsValid(obj);

		protected abstract T ResolveSceneObject([NotNull] String query);
		protected abstract Boolean IsValid(T value);

		public override String ToString() => _cachedObject != null && _cachedObject.TryGetTarget(out var target)
			? target.ToString()
			: $"{GetType().Name}(\"{_query}\")";
	}

	public sealed class LunyGameObjectRef : LunyRef<LunyGameObject>
	{
		public static implicit operator LunyGameObjectRef(String name) => new(name);
		public static implicit operator LunyGameObjectRef(LunyGameObject obj) => new(obj);
		public static implicit operator LunyGameObject(LunyGameObjectRef gameObjectRef) => gameObjectRef.Value;

		public LunyGameObjectRef(String name)
			: base(name) {}

		public LunyGameObjectRef(LunyGameObject obj)
			: base(obj) {}

		protected override LunyGameObject ResolveSceneObject(String query) => LunyEngine.Instance.Scene.FindObjectByName(query);
		protected override Boolean IsValid(LunyGameObject value) => value != null && value.IsValid;

		public Boolean TryResolveReference(out LunyGameObject obj) => IsCachedObjectValid(out obj);
	}

	/// <summary>
	/// Resolves a named child object under a specific parent <see cref="LunyGameObject"/> at runtime.
	/// Caches the resolved child as a weak reference to avoid preventing GC.
	/// Blocks should attempt resolution in their ctor and re-resolve in Execute if the cached ref is no longer valid.
	/// </summary>
	public sealed class LunyChildRef
	{
		private readonly WeakReference<LunyGameObject> _parent;
		private readonly String _childName;
		private WeakReference<LunyGameObject> _cachedChild;

		public LunyChildRef(LunyGameObject parent, String childName)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));
			if (String.IsNullOrEmpty(childName))
				throw new ArgumentException("Child name cannot be null or empty.", nameof(childName));

			_parent = new WeakReference<LunyGameObject>(parent);
			_childName = childName;
			_cachedChild = new WeakReference<LunyGameObject>(null);
		}

		/// <summary>
		/// Resolves the child object by name under the parent. Returns null if not found or parent is destroyed.
		/// </summary>
		public LunyGameObject Resolve()
		{
			if (_cachedChild.TryGetTarget(out var cached) && cached != null && cached.IsValid)
				return cached;
			if (!_parent.TryGetTarget(out var parent) || parent == null || !parent.IsValid)
				return null;

			var child = LunyEngine.Instance.Scene.FindChildByName(parent, _childName);
			_cachedChild = new WeakReference<LunyGameObject>(child);
			return child;
		}
	}

	public sealed class LunyAssetRef : LunyRef<LunyAsset>
	{
		public static implicit operator LunyAssetRef(String name) => new(name);
		public static implicit operator LunyAssetRef(LunyAsset asset) => new(asset);

		public LunyAssetRef(String query)
			: base(query) {}

		public LunyAssetRef(ILunyAsset asset)
			: base((LunyAsset)asset) {}

		protected override LunyAsset ResolveSceneObject(String query) => throw new NotImplementedException(nameof(ResolveSceneObject));
		protected override Boolean IsValid(LunyAsset value) => value != null;
	}
}
