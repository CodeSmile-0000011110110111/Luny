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

		// Specific component types
		Transform = 1000,
		Rigidbody = 1001,

		// Asset Types
		Material = 3000,
		Mesh = 3001,
		AudioClip = 3002,

		// Value types (non-Object)
		Color = 8000,
		AnimationCurve = 8001,

		Vector2 = 8010,
		Vector3 = 8011,

	}
}
