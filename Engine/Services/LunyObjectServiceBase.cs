using Luny.Engine.Bridge;
using System;

namespace Luny.Engine.Services
{
	/// <summary>
	/// Provides engine-agnostic methods for creating objects and primitives.
	/// </summary>
	public interface ILunyObjectService : ILunyEngineService
	{
		ILunyGameObject CreateEmpty(String name, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation, LunyVector3? scale);

		ILunyGameObject CreatePrimitive(String name, LunyPrimitiveType type, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		/// <summary>
		/// Creates a new object from a prefab bridge.
		/// </summary>
		ILunyGameObject CreateFromPrefab(ILunyPrefab prefab, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		ILunyGameObject Clone(ILunyGameObject original, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation, LunyVector3? scale);
	}

	public abstract class LunyObjectServiceBase : LunyEngineServiceBase, ILunyObjectService
	{
		public abstract ILunyGameObject CreateEmpty(String name, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		public abstract ILunyGameObject CreatePrimitive(String name, LunyPrimitiveType type, ILunyGameObject parent, LunyVector3? position,
			LunyQuaternion? rotation, LunyVector3? scale);

		public abstract ILunyGameObject CreateFromPrefab(ILunyPrefab prefab, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);

		public abstract ILunyGameObject Clone(ILunyGameObject original, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale);
	}
}
