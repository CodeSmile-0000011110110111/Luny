using System;
using System.Collections.Generic;
using System.Linq;

namespace Luny.Engine.Bridge
{
	internal interface ILunyObjectLifecycleInternal
	{
		void OnObjectCreated(ILunyGameObject lunyGameObject);
		void ScheduleNativeObjectDestruction(ILunyGameObject lunyGameObject);
		void OnObjectEnabled(ILunyGameObject lunyGameObject);
		void OnObjectDisabled(ILunyGameObject lunyGameObject);
	}

	/// <summary>
	/// Manages the lifecycle state transitions of LunyObjects, including deferred
	/// OnReady execution and structural changes (destruction).
	/// </summary>
	internal sealed class LunyObjectLifecycle : ILunyObjectLifecycleInternal
	{
		private Queue<ILunyGameObject> _pendingReady = new();
		private Queue<ILunyGameObject> _pendingDestroy = new();
		private Dictionary<LunyObjectId, ILunyGameObject> _pendingReadyWaitingForEnable = new();

		/// <summary>
		/// Queues an object for its OnReady event.
		/// </summary>
		public void OnObjectCreated(ILunyGameObject lunyGameObject)
		{
			if (lunyGameObject.IsEnabledInHierarchy)
				_pendingReady.Enqueue(lunyGameObject);
			else
				_pendingReadyWaitingForEnable[lunyGameObject.LunyObjectId] = lunyGameObject;
		}

		/// <summary>
		/// Queues an object for deferred destruction.
		/// </summary>
		public void ScheduleNativeObjectDestruction(ILunyGameObject lunyGameObject) => _pendingDestroy.Enqueue(lunyGameObject);

		/// <summary>
		/// Notifies the manager that an object's enabled state has changed.
		/// Used to move objects from the waiting queue to the ready queue.
		/// </summary>
		public void OnObjectEnabled(ILunyGameObject lunyGameObject)
		{
			if (_pendingReadyWaitingForEnable.Remove(lunyGameObject.LunyObjectId, out var obj))
				_pendingReady.Enqueue(obj);
		}

		public void OnObjectDisabled(ILunyGameObject lunyGameObject) {}

		~LunyObjectLifecycle() => LunyTraceLogger.LogInfoFinalized(this);

		public void OnEngineFrameBegins() => ProcessPendingReady();
		public void OnEngineFrameEnds() => ProcessPendingDestroy();

		private void ProcessPendingReady()
		{
			while (_pendingReady.Count > 0)
			{
				var obj = _pendingReady.Dequeue();
				if (obj is LunyGameObject lunyObjectImpl && lunyObjectImpl.IsValid)
					lunyObjectImpl.InvokeOnReady();
			}
		}

		private void ProcessPendingDestroy()
		{
			// if (_pendingDestroy.Count > 0)
			// 	LunyLogger.LogInfo($"Processing pending destroy queue: {_pendingDestroy.Count} objects", this);

			while (_pendingDestroy.Count > 0)
			{
				var obj = _pendingDestroy.Dequeue();
				if (obj is LunyGameObject lunyObjectImpl)
					lunyObjectImpl.DestroyNativeObjectInternal();
			}
		}

		public void Shutdown(LunyObjectRegistry objectRegistry)
		{
			LunyLogger.LogInfo("==== SHUTDOWN ====", this);

			// ensure all objects run their OnDestroy, must use a copy of collection because it will be modified
			var allObjects = objectRegistry.AllObjects.ToArray();
			foreach (var lunyObject in allObjects)
				lunyObject.Destroy();

			// cleans up any pending to be destroyed native objects
			ProcessPendingDestroy();

			_pendingReady.Clear();
			_pendingDestroy.Clear();
			_pendingReadyWaitingForEnable.Clear();
			_pendingReady = null;
			_pendingDestroy = null;
			_pendingReadyWaitingForEnable = null;
			GC.SuppressFinalize(this);
		}
	}
}
