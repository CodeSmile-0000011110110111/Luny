namespace Luny.Engine.Bridge
{
	public enum LunyInterpolation
	{
		Instant, // Teleport to target
		Towards, // Vector3.MoveTowards (Constant velocity)
		Linear, // Vector3.Lerp (Constant percentage)
		Spherical, // Vector3.Slerp (Arcing motion)
		LinearUnclamped, // Vector3.Lerp (Constant percentage)
		SphericalUnclamped, // Vector3.Slerp (Arcing motion)
		SmoothStep, // Lerp with Ease-In/Out
		SmoothDamp, // Vector3.SmoothDamp (Physics-based follow)
	}

	public enum LunyRigidbodyInterpolation
	{
		/// <summary>   No Interpolation. </summary>
		None,
		/// <summary>
		///   Interpolation will always lag a little bit behind (moves towards target position) but can be smoother than extrapolation.
		/// </summary>
		Interpolate,
		/// <summary>
		///   Extrapolation will predict the future position of the rigidbody based on the current velocity.
		/// </summary>
		Extrapolate,
	}
}
