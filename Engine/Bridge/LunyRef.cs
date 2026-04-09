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
				if (!_cachedObject.TryGetTarget(out var obj) || !IsValid(obj))
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

		protected abstract T ResolveSceneObject([NotNull] String query);
		protected abstract Boolean IsValid(T value);

		public override String ToString() => _cachedObject != null && _cachedObject.TryGetTarget(out var target)
			? target.ToString()
			: $"{GetType().Name}(\"{_query}\")";
	}

	public sealed class LunyObjectRef : LunyRef<LunyObject>
	{
		public static implicit operator LunyObjectRef(String name) => new(name);
		public static implicit operator LunyObjectRef(LunyObject obj) => new(obj);
		public static implicit operator LunyObject(LunyObjectRef objectRef) => objectRef.Value;

		public LunyObjectRef(String name)
			: base(name) {}

		public LunyObjectRef(ILunyObject obj)
			: base((LunyObject)obj) {}

		protected override LunyObject ResolveSceneObject(String query) => (LunyObject)LunyEngine.Instance.Scene.FindObjectByName(query);
		protected override Boolean IsValid(LunyObject value) => value != null && value.IsValid;
	}

	/// <summary>
	/// Resolves a named child object under a specific parent <see cref="ILunyObject"/> at runtime.
	/// Caches the resolved child as a weak reference to avoid preventing GC.
	/// Blocks should attempt resolution in their ctor and re-resolve in Execute if the cached ref is no longer valid.
	/// </summary>
	public sealed class LunyChildRef
	{
		private readonly WeakReference<ILunyObject> _parent;
		private readonly String _childName;
		private WeakReference<ILunyObject> _cachedChild;

		public LunyChildRef(ILunyObject parent, String childName)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));
			if (String.IsNullOrEmpty(childName))
				throw new ArgumentException("Child name cannot be null or empty.", nameof(childName));

			_parent = new WeakReference<ILunyObject>(parent);
			_childName = childName;
			_cachedChild = new WeakReference<ILunyObject>(null);
		}

		/// <summary>
		/// Resolves the child object by name under the parent. Returns null if not found or parent is destroyed.
		/// </summary>
		public ILunyObject Resolve()
		{
			if (_cachedChild.TryGetTarget(out var cached) && cached != null && cached.IsValid)
				return cached;
			if (!_parent.TryGetTarget(out var parent) || parent == null || !parent.IsValid)
				return null;

			var child = LunyEngine.Instance.Scene.FindChildByName(parent, _childName);
			_cachedChild = new WeakReference<ILunyObject>(child);
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
