using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luny
{
	public interface ILunyObjectRegistry
	{
		Int32 Count { get; }
		IEnumerable<ILunyGameObject> AllObjects { get; }
		Boolean TryGetByLunyID(LunyObjectId lunyObjectID, out ILunyGameObject lunyGameObject);
		Boolean TryGetByNativeId(LunyNativeObjectId lunyNativeObjectID, out ILunyGameObject lunyGameObject);
		ILunyGameObject GetCached(String objectName);
		ILunyGameObject Find(String objectName);
	}

	internal interface ILunyObjectRegistryInternal : ILunyObjectRegistry
	{
		void Register(ILunyGameObject lunyGameObject);
		Boolean Unregister(ILunyGameObject lunyGameObject);
	}

	/// <summary>
	/// Registry for tracking all active LunyObject instances.
	/// Provides O(1) lookup by both LunyID and NativeID.
	/// </summary>
	internal sealed class LunyObjectRegistry : ILunyObjectRegistryInternal
	{
		private Dictionary<LunyObjectId, ILunyGameObject> _objectsByLunyID = new();
		private Dictionary<LunyNativeObjectId, ILunyGameObject> _objectsByNativeID = new();

		/// <summary>
		/// Gets the total number of registered objects.
		/// </summary>
		public Int32 Count => _objectsByLunyID.Count;

		/// <summary>
		/// Gets all registered objects.
		/// </summary>
		public IEnumerable<ILunyGameObject> AllObjects => _objectsByLunyID.Values;

		/// <summary>
		/// Registers a new object. Throws if already registered.
		/// </summary>
		public void Register(ILunyGameObject lunyGameObject)
		{
			if (lunyGameObject == null)
				throw new ArgumentNullException(nameof(lunyGameObject));

			var lunyID = lunyGameObject.LunyObjectId;
			var nativeID = lunyGameObject.NativeObjectId;

#if DEBUG
			if (_objectsByLunyID.ContainsKey(lunyID))
				throw new InvalidOperationException($"Object with LunyID {lunyID} already registered.");
#endif

			_objectsByLunyID[lunyID] = lunyGameObject;
			_objectsByNativeID[nativeID] = lunyGameObject;

			((LunyEngine)LunyEngine.Instance).ObjectRegistered(lunyGameObject);
		}

		/// <summary>
		/// Unregisters an object.
		/// </summary>
		public Boolean Unregister(ILunyGameObject lunyGameObject)
		{
			if (lunyGameObject == null)
				return false;

			var removed = TryRemove(lunyGameObject.LunyObjectId);
			if (removed)
				((LunyEngine)LunyEngine.Instance).ObjectUnregistered(lunyGameObject);

#if DEBUG
			if (!removed)
				LunyLogger.LogWarning($"Tried to unregister non-existent LunyID {lunyGameObject.LunyObjectId}");
#endif

			return removed;
		}

		public ILunyGameObject GetCached(String objectName) => _objectsByLunyID.Values.FirstOrDefault(obj => obj.Name == objectName);

		public ILunyGameObject Find(String objectName)
		{
			var existing = GetCached(objectName);
			if (existing != null)
				return existing;

			var sceneObject = LunyEngine.Instance.Scene.FindObjectByName(objectName);
			if (sceneObject != null)
			{
				// sceneObject might have been already cached by the bridge (e.g. UnityGameObject.ToLunyObject)
				// check if it's already in our registries by its LunyID or NativeID
				if (TryGetByNativeId(sceneObject.NativeObjectId, out var registeredObject))
					return registeredObject;

				Register(sceneObject);
				return sceneObject;
			}

			// TODO: proxy fallback or auto-create if needed?
			// The task said "with proxy fallback" for Object.Create
			return null;
		}

		/// <summary>
		/// Finds an object by its NativeID.
		/// </summary>
		public Boolean TryGetByNativeId(LunyNativeObjectId lunyNativeObjectID, out ILunyGameObject lunyGameObject) =>
			_objectsByNativeID.TryGetValue(lunyNativeObjectID, out lunyGameObject);

		/// <summary>
		/// Finds an object by its LunyID.
		/// </summary>
		public Boolean TryGetByLunyID(LunyObjectId lunyObjectID, out ILunyGameObject lunyGameObject) =>
			_objectsByLunyID.TryGetValue(lunyObjectID, out lunyGameObject);

		/// <summary>
		/// Unregisters an object by its LunyID.
		/// </summary>
		private Boolean TryRemove(LunyObjectId lunyObjectID)
		{
			if (_objectsByLunyID.Remove(lunyObjectID, out var lunyObject))
			{
				_objectsByNativeID.Remove(lunyObject.NativeObjectId);
				return true;
			}
			return false;
		}

		~LunyObjectRegistry() => LunyTraceLogger.LogInfoFinalized(this);

		internal void Shutdown()
		{
			DestroyInvalidatedObjects();

			_objectsByLunyID.Clear();
			_objectsByNativeID.Clear();
			_objectsByLunyID = null;
			_objectsByNativeID = null;

			GC.SuppressFinalize(this);
		}

		internal void OnSceneUnloaded(ILunyScene unloadedScene) => DestroyInvalidatedObjects();

		private void DestroyInvalidatedObjects()
		{
			// TODO: avoid the list copy - Destroy() may modify AllObjects
			var allObjects = AllObjects.ToArray();
			foreach (var lunyObject in allObjects)
			{
				if (lunyObject.NativeObject == null)
				{
					LunyLogger.LogInfo($"{lunyObject} no longer valid, unregistering ...", this);
					Unregister(lunyObject);
				}
			}
		}
	}
}
