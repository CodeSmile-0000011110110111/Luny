using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using SystemObject = System.Object;

namespace Luny.Engine.Bridge
{
	/// <summary>
	/// See implementation: <see cref="LunyGameObject"/>
	/// </summary>
	public interface ILunyGameObject
	{
		/// <summary>
		/// Runs when the object was created or ownership was transferred to LunyEngine. Runs even if the object starts disabled.
		/// </summary>
		/// <remarks>
		/// Engine-native creation events (Unity: Awake, OnEnable / Godot: ctor, _init) will run before <see cref="OnCreated"/>.
		/// <see cref="OnEnabled"/> will run right after <see cref="OnCreated"/> if the object starts enabled.
		/// </remarks>
		public event Action OnCreated;
		/// <summary>
		/// Runs when the object was destroyed. Runs even if the object is disabled.
		/// </summary>
		/// <remarks>
		/// The object's <see cref="NativeObject"/> reference is still accessible, it has not been destroyed yet.
		/// </remarks>
		public event Action OnDestroyed;
		/// <summary>
		/// Runs once before the object's first frame processing, before OnFrameUpdate and OnHeartbeat.
		/// If the object starts disabled, OnReady runs after the object first gets enabled.
		/// </summary>
		/// <remarks>
		/// It is not guaranteed that the object will simulate physics in its first active frame. The event order is either:
		///		OnReady => OnFrameUpdate (object's first)
		/// or:
		///		OnReady => OnHeartbeat (object's first) => OnFrameUpdate (object's first)
		/// </remarks>
		public event Action OnReady;
		/// <summary>
		/// Runs when the object's enabled state changes to "enabled": visible, updating, receiving events, interacting with other objects.
		/// Also runs right after OnCreated if the object starts enabled.
		/// </summary>
		public event Action OnEnabled;
		/// <summary>
		/// Runs when the object's enabled state changes to "disabled": hidden, not updating, not receiving events, not interacting with other objects.
		/// </summary>
		public event Action OnDisabled;

		/// <summary>
		/// Runs when the object's collider has "entered" (overlaps) another static or non-kinematic collider.
		/// </summary>
		public event Action<LunyCollision> OnCollisionEntered;
		/// <summary>
		/// Runs when the object's collider stops overlapping/touching another static or non-kinematic collider.
		/// </summary>
		public event Action<LunyCollision> OnCollisionExited;
		/// <summary>
		/// Runs every heartbeat while the collision is ongoing.
		/// </summary>
		public event Action<LunyCollision> OnCollisionUpdate;
		/// <summary>
		/// Runs when first overlapping a trigger collider.
		/// </summary>
		public event Action<LunyCollider> OnTriggerEntered;
		/// <summary>
		/// Runs when leaving a trigger collider.
		/// </summary>
		public event Action<LunyCollider> OnTriggerExited;
		/// <summary>
		/// Runs while overlapping a trigger collider.
		/// </summary>
		/// <remarks>
		/// In Unity, to receive this event you have to explicitly enable it:
		/// **Project Settings**: Physics/Settings -> GameObject -> Generate On Trigger Stay Events
		/// </remarks>
		public event Action<LunyCollider> OnTriggerUpdate;
		public event Action<LunyCollision2D> OnCollisionEntered2D;
		public event Action<LunyCollision2D> OnCollisionExited2D;
		public event Action<LunyCollision2D> OnCollisionUpdate2D;
		public event Action<LunyCollider2D> OnTriggerEntered2D;
		public event Action<LunyCollider2D> OnTriggerExited2D;
		public event Action<LunyCollider2D> OnTriggerUpdate2D;

		/// <summary>
		/// Unique, immutable identifier for LunyObject. This ID is distinct from engine's native object ID!
		/// </summary>
		LunyObjectId LunyObjectId { get; }
		/// <summary>
		/// Engine-native object's unique, immutable identifier. Subject to engine's behaviour (ie may change between runs).
		/// </summary>
		/// <remarks>To aid debugging, this ID remains valid after the engine-native object has been destroyed.</remarks>
		LunyNativeObjectId NativeObjectId { get; }
		/// <summary>
		/// Gets the underlying engine-native object (GameObject, Node) as generic System.Object type.
		/// Use the <see cref="Cast{T}"/> or <see cref="As{T}"/> methods to avoid manually casting the reference.
		/// </summary>
		SystemObject NativeObject { get; }
		/// <summary>
		/// The <see cref="Luny.Engine.Bridge.LunyTransform"/> of this object.
		/// </summary>
		LunyTransform Transform { get; }
		/// <summary>
		/// The <see cref="LunyRigidbody"/> of this object. Returns null if no rigidbody component exists.
		/// </summary>
		ILunyRigidbody Rigidbody { get; }
		/// <summary>
		/// The name of the object in the scene hierarchy.
		/// </summary>
		/// <remarks>To aid debugging, this property remains valid even after the object has been destroyed.</remarks>
		String Name { get; set; }
		/// <summary>
		/// Whether the object and its native representation are valid (not null, not destroyed).
		/// </summary>
		Boolean IsValid { get; }
		/// <summary>
		/// Is true when Destroy() was called on the NativeObject and it was set to null.
		/// </summary>
		Boolean IsNativeObjectValid { get; }
		/// <summary>
		/// Whether the engine object is processing and visible. Matches the "Active" state of Unity.
		/// </summary>
		/// <remarks>
		/// The object may be enabled, but still be disabled in the hierarchy due to a parent not being enabled.
		/// IsEnabled also toggles visibility. If the object's IsVisible was set to false, and then the object gets enabled,
		/// the object's IsVisible state will also change to true.
		/// </remarks>
		Boolean IsEnabled { get; set; }
		/// <summary>
		/// Returns true only if BOTH the object itself AND all of its parents are enabled. Otherwise returns false.
		/// </summary>
		Boolean IsEnabledInHierarchy { get; }
		/// <summary>
		/// Whether the object is visible (will render).
		/// </summary>
		/// <remarks>
		/// This property does NOT imply that the object is currently visible on screen. It is technically visible, but may still
		/// be outside the camera's frame, obstructed by other objects, fully transparent, scaled infinitely small, etc.
		/// </remarks>
		Boolean IsVisible { get; set; }

		/// <summary>
		/// Gets the engine-native object as type T. Returns null for non-matching types.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T As<T>() where T : class;

		/// <summary>
		/// Gets the engine-native object cast to T. Throws if the type cast is invalid.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Cast<T>() where T : class;

		/// <summary>
		/// Marks this object for destruction. If object is enabled, will run its OnDisabled event. Then it run its OnDestroyed event.
		/// </summary>
		/// <remarks>The engine-native object destruction is deferred until the end of the current frame to prevent exceptions.</remarks>
		void Destroy();

		/// <summary>
		/// Creates a new instance of the current object.
		/// </summary>
		/// <returns></returns>
		ILunyGameObject Clone();

		/// <summary>
		/// Creates a new instance of the current object and parents it.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		ILunyGameObject Clone(LunyTransform parent);
	}

	/// <summary>
	/// Engine-agnostic wrapper for engine objects.
	/// Safeguards against NullReferenceExceptions when the engine-native object may have been destroyed.
	/// </summary>
	public abstract class LunyGameObject : ILunyGameObject
	{
		public event Action OnCreated;
		public event Action OnDestroyed;
		public event Action OnReady;
		public event Action OnEnabled;
		public event Action OnDisabled;
		public event Action<LunyCollision> OnCollisionEntered;
		public event Action<LunyCollision> OnCollisionExited;
		public event Action<LunyCollision> OnCollisionUpdate;
		public event Action<LunyCollider> OnTriggerEntered;
		public event Action<LunyCollider> OnTriggerExited;
		public event Action<LunyCollider> OnTriggerUpdate;
		public event Action<LunyCollision2D> OnCollisionEntered2D;
		public event Action<LunyCollision2D> OnCollisionExited2D;
		public event Action<LunyCollision2D> OnCollisionUpdate2D;
		public event Action<LunyCollider2D> OnTriggerEntered2D;
		public event Action<LunyCollider2D> OnTriggerExited2D;
		public event Action<LunyCollider2D> OnTriggerUpdate2D;

		private readonly LunyObjectId _lunyObjectId;
		private readonly LunyNativeObjectId _nativeObjectId;
		private SystemObject _nativeObject;
		private ObjectState _state;

		[NotNull] private static ILunyObjectLifecycleInternal Lifecycle => ((ILunyEngineInternal)LunyEngine.Instance).ObjectLifecycle;
		[NotNull] private static ILunyObjectRegistryInternal Objects => (ILunyObjectRegistryInternal)LunyEngine.Instance.Objects;

		public LunyObjectId LunyObjectId => _lunyObjectId;
		public LunyNativeObjectId NativeObjectId => _nativeObjectId;
		public SystemObject NativeObject => _nativeObject;
		/// <remarks>
		/// Caching is handled inside <see cref="GetNativeTransform"/> (engine-specific subclass).
		/// The subclass must null the cache when the native object is destroyed to allow GC.
		/// TODO: refactor to GetNativeComponent&lt;T&gt;() once LunyComponent base class exists.
		/// </remarks>
		public LunyTransform Transform => IsValid ? GetNativeTransform() : null;
		/// <remarks>
		/// Caching is handled inside <see cref="GetNativeRigidbody"/> (engine-specific subclass).
		/// Returns null if the native object has no rigidbody component or has been destroyed.
		/// TODO: refactor to GetNativeComponent&lt;T&gt;() once LunyComponent base class exists.
		/// </remarks>
		public LunyRigidbody Rigidbody => IsValid ? GetNativeRigidbody() : null;
		ILunyRigidbody ILunyGameObject.Rigidbody => Rigidbody;

#if DEBUG || LUNY_DEBUG
		private String DebugNativeObjectName { get; set; }
#else
		private String DebugNativeObjectName { get => String.Empty; set {} }
#endif

		public String Name
		{
			get => IsValid ? GetNativeObjectName() : $"{Emoji.Destroyed}{_nativeObjectId}: \"{DebugNativeObjectName}\"";
			set
			{
				if (IsValid)
				{
					SetNativeObjectName(value);
#if DEBUG || LUNY_DEBUG
					// engine may modify names we set, thus get it from the engine, not 'value'
					DebugNativeObjectName = GetNativeObjectName();
#endif
				}
			}
		}

		public Boolean IsValid => !IsNativeObjectValid && IsNativeObjectReferenceValid();
		public Boolean IsNativeObjectValid => _state.IsDestroyed || _state.IsDestroying;

		public Boolean IsEnabled
		{
			get => _state.IsEnabled && IsValid;
			set
			{
				if (_state.IsEnabled != value && IsValid)
					SetEnabledState(value);
			}
		}

		public Boolean IsEnabledInHierarchy => _state.IsEnabled && IsValid && GetNativeObjectEnabledInHierarchy();
		public Boolean IsVisible
		{
			get => _state.IsVisible && IsValid;
			set
			{
				if (_state.IsVisible != value && IsValid)
					SetVisibleState(value);
			}
		}

		private LunyGameObject() {} // Hidden ctor

		/// <summary>
		/// Instantiates a LunyObject instance.
		/// </summary>
		protected LunyGameObject(SystemObject nativeObject, Int64 nativeObjectId, Boolean isNativeObjectEnabled, Boolean isNativeObjectVisible)
		{
			if (nativeObject == null)
				throw new LunyBridgeException($"{this}: {nameof(LunyGameObject)} created with a <null> reference");

			_state.IsEnabled = isNativeObjectEnabled;
			_state.IsVisible = isNativeObjectVisible;
			_nativeObject = nativeObject;
			_nativeObjectId = nativeObjectId;
			_lunyObjectId = LunyObjectId.Generate();
			Objects.Register(this);
		}

		protected static Boolean TryGetCached(Int64 nativeId, out ILunyGameObject lunyGameObject) => Objects.TryGetByNativeId(nativeId, out lunyGameObject);

		public T As<T>() where T : class => _nativeObject as T;
		public T Cast<T>() where T : class => (T)_nativeObject;

		public void Initialize()
		{
			ThrowIfInitializedAgain();
			_state.IsInitialized = true;

			DebugNativeObjectName = GetNativeObjectName();

			Lifecycle.OnObjectCreated(this);
			OnCreated?.Invoke();

			SetVisibleState(_state.IsVisible);

			// bypassing property is intentional
			if (_state.IsEnabled)
				SetEnabledState(_state.IsEnabled); // will trigger OnEnable
		}

		public void Destroy()
		{
			if (_state.IsDestroying || _state.IsDestroyed)
				return;

			//LunyLogger.LogInfo($"Destroying ... {this} ({GetHashCode()})", this);

			// prevents re-entry from other On.Disabled/On.Destroyed event blocks which might run Object.Destroy()
			_state.IsDestroying = true;

			// bypassing property is intentional (IsValid is false now)
			if (_state.IsEnabled)
				SetEnabledState(false);

			var onDestroyEvent = OnDestroyed;
			ClearObjectEvents();
			onDestroyEvent?.Invoke();

			if (_nativeObject != null)
				Lifecycle.ScheduleNativeObjectDestruction(this);
			else
				DestroyNativeObjectInternal(); // to satisfy the pattern (suppress finalizer)

			Objects.Unregister(this);
		}

		public abstract ILunyGameObject Clone();

		public abstract ILunyGameObject Clone(LunyTransform parent);

		private void ClearObjectEvents()
		{
			OnCreated = null;
			OnEnabled = null;
			OnDisabled = null;
			OnReady = null;
			OnDestroyed = null;
			OnCollisionEntered = null;
			OnCollisionExited = null;
			OnCollisionUpdate = null;
			OnTriggerEntered = null;
			OnTriggerExited = null;
			OnTriggerUpdate = null;
			OnCollisionEntered2D = null;
			OnCollisionExited2D = null;
			OnCollisionUpdate2D = null;
			OnTriggerEntered2D = null;
			OnTriggerExited2D = null;
			OnTriggerUpdate2D = null;
		}

		// Should only be called internally by LunyObjectLifecycleManager from pending destroy queue processing
		internal void DestroyNativeObjectInternal()
		{
			if (!_state.IsDestroying)
				throw new LunyLifecycleException($"{this}: {nameof(DestroyNativeObjectInternal)}() called without prior {nameof(Destroy)}()");

			if (_nativeObject != null)
			{
				//LunyLogger.LogInfo($"Destroying native object: {_nativeObject}", this);
				DestroyNativeObject();
				_nativeObject = null;
			}

			_state.IsDestroyed = true;
			GC.SuppressFinalize(this);

			//LunyLogger.LogInfo($"Destroyed: {this} ({GetHashCode()})", this);
		}

		~LunyGameObject() => LunyTraceLogger.LogInfoFinalized(this);

		private void SetVisibleState(Boolean visible)
		{
			_state.IsVisible = visible;

			if (visible)
				SetNativeObjectVisible();
			else
				SetNativeObjectInvisible();
		}

		private void SetEnabledState(Boolean enabled)
		{
			_state.IsEnabled = enabled;

			if (enabled)
			{
				SetNativeObjectEnabled();
				Lifecycle.OnObjectEnabled(this);
				OnEnabled?.Invoke();
			}
			else
			{
				SetNativeObjectDisabled();
				Lifecycle.OnObjectDisabled(this);
				OnDisabled?.Invoke();
			}
		}

		// LunyObjectLifecycleManager calls this
		internal void InvokeOnReady()
		{
			ThrowIfAlreadyReady();

			_state.IsReady = true;
			OnReady?.Invoke();
		}

		protected abstract LunyTransform GetNativeTransform();
		protected abstract LunyRigidbody GetNativeRigidbody();
		protected abstract void DestroyNativeObject();
		protected abstract Boolean IsNativeObjectReferenceValid();
		protected abstract String GetNativeObjectName();
		protected abstract void SetNativeObjectName(String name);
		protected abstract Boolean GetNativeObjectEnabledInHierarchy();
		protected abstract Boolean GetNativeObjectEnabled();
		protected abstract void SetNativeObjectEnabled();
		protected abstract void SetNativeObjectDisabled();
		protected abstract void SetNativeObjectVisible();
		protected abstract void SetNativeObjectInvisible();

		public override String ToString() => IsValid ? $"{Emoji.IsEnabled(IsEnabled)}\"{Name}\" ({LunyObjectId}, {NativeObjectId})" : Name;

		[Conditional("DEBUG")] [Conditional("LUNY_DEBUG")]
		private void ThrowIfInitializedAgain()
		{
#if DEBUG || LUNY_DEBUG
			if (_state.IsInitialized)
				throw new LunyLifecycleException($"{this} has already been initialized!");
#endif
		}

		[Conditional("DEBUG")] [Conditional("LUNY_DEBUG")]
		private void ThrowIfAlreadyReady()
		{
#if DEBUG || LUNY_DEBUG
			if (_state.IsReady)
				throw new LunyLifecycleException($"{this} is already Ready!");
#endif
		}

		private struct ObjectState
		{
			[Flags]
			private enum StateFlags
			{
				Initialized = 1 << 0,
				Ready = 1 << 1,
				Destroying = 1 << 2,
				Destroyed = 1 << 3,
				Enabled = 1 << 4,
				Visible = 1 << 5,
			}

			private StateFlags _flags;

			public Boolean IsInitialized
			{
				get => (_flags & StateFlags.Initialized) != 0;
				set => SetFlag(StateFlags.Initialized, value);
			}

			public Boolean IsEnabled
			{
				get => (_flags & StateFlags.Enabled) != 0;
				set => SetFlag(StateFlags.Enabled, value);
			}

			public Boolean IsVisible
			{
				get => (_flags & StateFlags.Visible) != 0;
				set => SetFlag(StateFlags.Visible, value);
			}

			public Boolean IsDestroyed
			{
				get => (_flags & StateFlags.Destroyed) != 0;
				set => SetFlag(StateFlags.Destroyed, value);
			}

			public Boolean IsDestroying
			{
				get => (_flags & StateFlags.Destroying) != 0;
				set => SetFlag(StateFlags.Destroying, value);
			}

			public Boolean IsReady
			{
				get => (_flags & StateFlags.Ready) != 0;
				set => SetFlag(StateFlags.Ready, value);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void SetFlag(StateFlags flag, Boolean value)
			{
				if (value)
					_flags |= flag;
				else
					_flags &= ~flag;
			}

			public override String ToString()
			{
				var sb = new StringBuilder("(");
				var first = true;

				if (IsDestroying)
				{
					sb.Append(nameof(StateFlags.Destroying));
					first = false;
				}
				if (IsDestroyed)
				{
					AppendSeparatorIfNeeded();
					sb.Append(nameof(StateFlags.Destroyed));
					first = false;
				}
				if (IsInitialized)
				{
					AppendSeparatorIfNeeded();
					sb.Append(nameof(StateFlags.Initialized));
					first = false;
				}
				if (IsReady)
				{
					AppendSeparatorIfNeeded();
					sb.Append(nameof(StateFlags.Ready));
					first = false;
				}
				if (IsEnabled)
				{
					AppendSeparatorIfNeeded();
					sb.Append(nameof(StateFlags.Enabled));
					first = false;
				}
				if (IsVisible)
				{
					AppendSeparatorIfNeeded();
					sb.Append(nameof(StateFlags.Visible));
					first = false;
				}

				sb.Append(")");
				return sb.ToString();

				void AppendSeparatorIfNeeded()
				{
					if (!first)
						sb.Append("|");
				}
			}
		}

		public void InvokeOnCollisionEntered(LunyCollision collision) => OnCollisionEntered?.Invoke(collision);
		public void InvokeOnCollisionExited(LunyCollision collision) => OnCollisionExited?.Invoke(collision);
		public void InvokeOnCollisionUpdate(LunyCollision collision) => OnCollisionUpdate?.Invoke(collision);
		public void InvokeOnTriggerEntered(LunyCollider collider) => OnTriggerEntered?.Invoke(collider);
		public void InvokeOnTriggerExited(LunyCollider collider) => OnTriggerExited?.Invoke(collider);
		public void InvokeOnTriggerUpdate(LunyCollider collider) => OnTriggerUpdate?.Invoke(collider);
		public void InvokeOnCollisionEntered2D(LunyCollision2D collision) => OnCollisionEntered2D?.Invoke(collision);
		public void InvokeOnCollisionExited2D(LunyCollision2D collision) => OnCollisionExited2D?.Invoke(collision);
		public void InvokeOnCollisionUpdate2D(LunyCollision2D collision) => OnCollisionUpdate2D?.Invoke(collision);
		public void InvokeOnTriggerEntered2D(LunyCollider2D collider) => OnTriggerEntered2D?.Invoke(collider);
		public void InvokeOnTriggerExited2D(LunyCollider2D collider) => OnTriggerExited2D?.Invoke(collider);
		public void InvokeOnTriggerUpdate2D(LunyCollider2D collider) => OnTriggerUpdate2D?.Invoke(collider);
	}
}
