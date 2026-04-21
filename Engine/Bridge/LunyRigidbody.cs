using System;

namespace Luny.Engine.Bridge
{
	/// <summary>
	/// See implementation: <see cref="LunyRigidbody"/>
	/// </summary>
	public interface ILunyRigidbody
	{
		/// <summary>The owning <see cref="ILunyGameObject"/>. May return null if the owner has been destroyed.</summary>
		ILunyGameObject Owner { get; }

		/// <summary>Sets whether the rigidbody is kinematic (not affected by physics forces).</summary>
		Boolean IsKinematic { get; set; }

		/// <summary>Sets whether gravity affects this rigidbody.</summary>
		Boolean UseGravity { get; set; }

		/// <summary>
		/// Sets the rigidbody's interpolation mode.
		/// </summary>
		LunyRigidbodyInterpolation Interpolation { get; set; }

		/// <summary>
		/// Moves the rigidbody by <paramref name="delta"/> this physics step.
		/// Local space by default; pass <see cref="LunyTransformSpace.World"/> for world space.
		/// </summary>
		void MovePosition(LunyVector3 delta, LunyTransformSpace space);

		/// <summary>
		/// Rotates the rigidbody by <paramref name="eulerDelta"/> degrees this physics step.
		/// Local space by default; pass <see cref="LunyTransformSpace.World"/> for world space.
		/// </summary>
		void MoveRotation(LunyVector3 eulerDelta, LunyTransformSpace space);

		/// <summary>
		/// Applies a linear force or impulse to the rigidbody.
		/// Local space by default; pass <see cref="LunyTransformSpace.World"/> for world space.
		/// </summary>
		void AddForce(LunyVector3 force, LunyForceMode forceMode, LunyTransformSpace space);

		/// <summary>
		/// Applies a linear force or impulse at a specific world-space position, generating torque.
		/// The caller is responsible for converting any local offset to world space before calling.
		/// </summary>
		void AddForceAtPosition(LunyVector3 force, LunyVector3 worldPosition, LunyForceMode forceMode);

		/// <summary>
		/// Applies an angular force (torque) or angular impulse to the rigidbody.
		/// Local space by default; pass <see cref="LunyTransformSpace.World"/> for world space.
		/// </summary>
		void AddTorque(LunyVector3 torque, LunyForceMode forceMode, LunyTransformSpace space);
	}

	/// <summary>
	/// Engine-agnostic proxy for native rigidbody types (UnityEngine.Rigidbody, etc.).
	/// </summary>
	/// <remarks>
	/// TODO: refactor component caching (Transform, Rigidbody) to a GetNativeComponent&lt;T&gt;()
	/// pattern once a LunyComponent base class exists and more than two component types are supported.
	/// </remarks>
	public abstract class LunyRigidbody : ILunyRigidbody
	{
		private readonly WeakReference<ILunyGameObject> _owner;

		/// <inheritdoc/>
		public ILunyGameObject Owner => _owner.TryGetTarget(out var owner) ? owner : null;

		/// <inheritdoc/>
		public abstract Boolean IsKinematic { get; set; }

		/// <inheritdoc/>
		public abstract Boolean UseGravity { get; set; }
		public abstract LunyRigidbodyInterpolation Interpolation { get; set; }

		protected LunyRigidbody(ILunyGameObject owner) => _owner = new WeakReference<ILunyGameObject>(owner);

		/// <inheritdoc/>
		public abstract void MovePosition(LunyVector3 delta, LunyTransformSpace space);

		/// <inheritdoc/>
		public abstract void MoveRotation(LunyVector3 eulerDelta, LunyTransformSpace space);

		/// <inheritdoc/>
		public abstract void AddForce(LunyVector3 force, LunyForceMode forceMode, LunyTransformSpace space);

		/// <inheritdoc/>
		public abstract void AddForceAtPosition(LunyVector3 force, LunyVector3 worldPosition, LunyForceMode forceMode);

		/// <inheritdoc/>
		public abstract void AddTorque(LunyVector3 torque, LunyForceMode forceMode, LunyTransformSpace space);
	}
}
