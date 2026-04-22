using System;

namespace Luny
{
	public record EngineReference
	{
		public String Name;
		public Object Value;
		public EngineReferenceType Type;
	}

	public enum EngineReferenceType
	{
		// Base object types
		Object = 0,
		GameObject = 1,
		ScriptableObject = 2,
		Component = 3,

		// Value types (non-Object)
		Color = 100,
		AnimationCurve = 101,

		Vector2 = 110,
		Vector3 = 111,

		// Specific component types
		Transform = 500,
		Rigidbody = 501,

		// Assets
		Material = 1000,
		Mesh = 1001,
		AudioClip = 1002,
	}
}
