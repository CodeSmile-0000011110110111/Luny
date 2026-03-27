namespace Luny.Engine.Bridge
{
	/// <summary>
	/// LunyObject lifecycle events.
	/// </summary>
	public enum LunyObjectEvent
	{
		/// <summary>
		/// Runs immediately after object was instantiated.
		/// Unity: Awake | Godot: _init
		/// </summary>
		Created,
		/// <summary>
		/// Runs at the end of the frame in which object was requested to be destroyed.
		/// Object is disabled and hidden between request to destroy and actual destruction.
		/// Unity: OnDestroy | Godot: N/A
		/// </summary>
		Destroyed,

		/// <summary>
		/// Runs once per lifetime, just before the first call to either OnFixedStep (if it runs in this frame) or OnUpdate.
		/// Unity: Start | Godot: _ready
		/// </summary>
		Ready,

		/// <summary>
		/// Runs immediately after object was instantiated (right after OnCreate) or when a disabled object gets enabled.
		/// The object is technically already enabled during this event.
		/// Unity: OnEnable | Godot: _enter_tree
		/// </summary>
		Enabled,
		/// <summary>
		/// Runs immediately when object is requested to be destroyed, or when a enabled object gets disabled.
		/// The object is technically already disabled during this event.
		/// Unity: OnDisable | Godot: _exit_tree
		/// </summary>
		Disabled,

		// OnPaused,		// not updating
		// OnResumed,		// updating again
		// OnHidden,		// not rendering
		// OnVisible,		// rendering again

		/// <summary>
		/// Runs in sync with engine's fixed update / physics processing at a fixed rate.
		/// May run multiple times in a single frame: catch-up behaviour, particularly in low framerate / high fixed tick scenarios.
		/// Is not guaranteed to run before every OnUpdate, as fixed time step is often lower than framerate eg a 30 or 50 Hz fixed timestep is common, but framerates are usually 60 Hz or more.
		/// CAUTION: Unsuitable for Input Event handling! Input events may be missed, especially first-frame triggers.
		/// Unity: FixedUpdate | Godot: _physics_process
		/// </summary>
		Heartbeat,
		/// <summary>
		/// Runs in sync with engine's update.
		/// Unity: Update | Godot: _process
		/// </summary>
		FrameUpdate,
		/// <summary>
		/// Runs in sync with engine's update, directly after OnUpdate.
		/// Unity: LateUpdate | Godot: N/A
		/// </summary>
		AfterFrameUpdate,
	}
}
