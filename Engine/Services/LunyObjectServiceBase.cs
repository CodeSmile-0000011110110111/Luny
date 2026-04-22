using Luny.Engine.Bridge;
using System;

namespace Luny.Engine.Services
{
	/// <summary>
	/// Provides engine-agnostic methods for creating objects and primitives.
	/// </summary>
	public interface ILunyObjectService : ILunyEngineService
	{
		LunyGameObject CreateEmpty(String name, LunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation, LunyVector3? scale);

		LunyGameObject CreatePrimitive(String name, LunyPrimitiveType type, LunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		/// <summary>
		/// Creates a new object from a prefab bridge.
		/// </summary>
		LunyGameObject CreateFromPrefab(ILunyPrefab prefab, LunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		LunyGameObject Clone(LunyGameObject original, LunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation, LunyVector3? scale);
	}

	public abstract class LunyObjectServiceBase : LunyEngineServiceBase, ILunyObjectService
	{
		public abstract LunyGameObject CreateEmpty(String name, LunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		public abstract LunyGameObject CreatePrimitive(String name, LunyPrimitiveType type, LunyGameObject parent, LunyVector3? position,
			LunyQuaternion? rotation, LunyVector3? scale);

		public abstract LunyGameObject CreateFromPrefab(ILunyPrefab prefab, LunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		public abstract LunyGameObject Clone(LunyGameObject original, LunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);
	}
}
