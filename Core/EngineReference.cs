using System;

namespace Luny
{
	public record EngineReference
	{
		public String Name;
		public Object Value;
		public Int64 NativeId;
		public Boolean IsSceneReference;

		// TODO: this should provide getters for LunyGameObject
	}
}
