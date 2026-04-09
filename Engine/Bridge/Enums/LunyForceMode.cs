namespace Luny.Engine.Bridge
{
	/// <summary>
	/// Engine-agnostic equivalent of Unity's ForceMode (and similar concepts in other engines).
	/// Used by <see cref="ILunyRigidbody"/> methods to specify how a force or impulse is applied.
	/// </summary>
	/// <remarks>
	/// Unity mapping:
	/// <list type="bullet">
	/// <item><see cref="Force"/> → ForceMode.Force</item>
	/// <item><see cref="Acceleration"/> → ForceMode.Acceleration</item>
	/// <item><see cref="Impulse"/> → ForceMode.Impulse</item>
	/// <item><see cref="VelocityChange"/> → ForceMode.VelocityChange</item>
	/// </list>
	/// </remarks>
	public enum LunyForceMode
	{
		/// <summary>Continuous force, affected by mass.</summary>
		Force,
		/// <summary>Continuous force, ignores mass (pure acceleration).</summary>
		Acceleration,
		/// <summary>Instant velocity change, affected by mass.</summary>
		Impulse,
		/// <summary>Instant velocity change, ignores mass.</summary>
		VelocityChange,
	}
}
