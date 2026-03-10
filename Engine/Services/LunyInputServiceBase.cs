using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace Luny.Engine.Services
{
	/// <summary>
	/// Engine-agnostic input service providing event-driven and polling access to input actions.
	/// </summary>
	/// <remarks>
	/// IMPORTANT: Implementations must inherit from both the ILunyInputService interface and its corresponding LunyInputServiceBase class!
	/// </remarks>
	public interface ILunyInputService : ILunyEngineService
	{
		/// <summary>
		/// Fired when any enabled action changes value.
		/// </summary>
		public event Action<LunyInputActionEvent> OnInputAction;

		/// <summary>
		/// Gets last known axis vector for the named action.
		/// </summary>
		LunyVector2 GetDirection(String actionName);

		/// <summary>
		/// Gets last known axis vector as rotation (quaternion). Assumes positive Y is up vector.
		/// </summary>
		LunyQuaternion GetRotation(String actionName);

		/// <summary>
		/// Gets last known axis vector as rotation (quaternion) with custom up vector.
		/// </summary>
		LunyQuaternion GetRotation(String actionName, LunyVector3 worldUp);

		/// <summary>
		/// Gets the analog trigger value (0.0–1.0) for the named button action.
		/// </summary>
		Single GetAxis(String actionName);

		/// <summary>
		/// True only on the frame the button transitioned to pressed.
		/// </summary>
		Boolean GetButtonJustPressed(String actionName);

		/// <summary>
		/// True while button is held.
		/// </summary>
		Boolean GetButtonPressed(String actionName);

		/// <summary>
		/// Returns a button's pressed strength. Used by triggers configured to act like buttons.
		/// </summary>
		Single GetButtonStrength(String actionName);

		/// <summary>
		/// Activate a given control scheme eg "Gamepad" or "Keyboard&Mouse" to prevent other device input from coming through.
		/// </summary>
		/// <param name="schemeName"></param>
		void SetControlSchemes(params String[] schemeNames);

		void AssignUserToLastDevice(String userName, Int32 deviceId, ILunyObject lunyObject);
		void UnassignUser(String userName);
		Boolean IsUserPairedWithDevice(String userName, Int32 deviceId);
	}

	public abstract class LunyInputServiceBase : LunyEngineServiceBase, ILunyInputService
	{
		public event Action<LunyInputActionEvent> OnInputAction;

		private readonly Dictionary<String, LunyVector2> _directionVectors = new();
		private readonly Dictionary<String, Single> _axisValues = new();
		private readonly Dictionary<String, Single> _buttonStrengthValues = new();
		private readonly Dictionary<String, Boolean> _buttonPressed = new();
		private readonly Dictionary<String, Boolean> _buttonJustPressed = new();

		private readonly Dictionary<String, LunyInputActionEvent> _activeInputEvents = new();

		protected LunyInputActionEvent LastInputEvent { get; private set; }

		public LunyVector2 GetDirection(String actionName) => _directionVectors.TryGetValue(actionName, out var v) ? v : default;

		public LunyQuaternion GetRotation(String actionName) => _directionVectors.TryGetValue(actionName, out var v) && v != LunyVector2.Zero
			? LunyQuaternion.Euler(v.X, 0f, v.Y)
			: LunyQuaternion.Identity;

		public LunyQuaternion GetRotation(String actionName, LunyVector3 worldUp) =>
			_directionVectors.TryGetValue(actionName, out var v) && v != LunyVector2.Zero
				? LunyQuaternion.LookRotation(new LunyVector3(v.X, 0f, v.Y), worldUp)
				: LunyQuaternion.Identity;

		public Single GetAxis(String actionName) => _axisValues.TryGetValue(actionName, out var v) ? v : 0f;
		public Single GetButtonStrength(String actionName) => _buttonStrengthValues.TryGetValue(actionName, out var v) ? v : 0f;
		public Boolean GetButtonPressed(String actionName) => _buttonPressed.TryGetValue(actionName, out var v) && v;
		public Boolean GetButtonJustPressed(String actionName) => _buttonJustPressed.TryGetValue(actionName, out var v) && v;

		public abstract void SetControlSchemes(params String[] schemeNames);
		public abstract void AssignUserToLastDevice(String userName, Int32 deviceId, ILunyObject lunyObject);
		public abstract void UnassignUser(String userName);
		public abstract Boolean IsUserPairedWithDevice(String userName, Int32 deviceId);

		protected void SetDirectionalInput(String actionName, LunyVector2 value) => _directionVectors[actionName] = value;

		protected void SetAxisInput(String actionName, Single value) => _axisValues[actionName] = value;

		protected void SetButtonInput(String actionName, Boolean pressed, Single strength = 1f) {}

		protected LunyInputActionEvent GetOrCreateInputActionEvent(String actionName)
		{
			if (!_activeInputEvents.TryGetValue(actionName, out var evt))
				_activeInputEvents[actionName] = evt = new LunyInputActionEvent();

			return evt;
		}

		protected void HandleInputActionEvent(LunyInputActionEvent inputEvent)
		{
			//LunyLogger.LogInfo($"[{inputEvent.EventFrame}] {inputEvent.ActionName} phase {inputEvent.Phase}", this);
			LastInputEvent = inputEvent;
			OnInputAction?.Invoke(inputEvent);
		}

		protected override void OnServiceFrameUpdate()
		{
			foreach (var inputEvent in _activeInputEvents.Values)
			{
				var phase = inputEvent.Phase;
				if (phase == LunyInputActionPhase.Performed || phase == LunyInputActionPhase.Started)
					inputEvent.Phase = LunyInputActionPhase.Performing;

				if (inputEvent.Phase == LunyInputActionPhase.Performing)
					OnInputAction?.Invoke(inputEvent);
			}
		}

		/// <summary>
		/// Clears per-frame transition flags. Called at the start of each frame via OnServicePreUpdate.
		/// </summary>
		protected override void OnServicePostUpdate()
		{
			LastInputEvent = null;
			_buttonJustPressed.Clear();
		}
	}
}
