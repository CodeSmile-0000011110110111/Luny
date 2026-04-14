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
}
