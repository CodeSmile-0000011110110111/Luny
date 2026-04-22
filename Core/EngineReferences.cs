using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace Luny
{
	public interface IEngineReferences {}

	public sealed class EngineReferences : IEngineReferences
	{
		private Dictionary<String, EngineReference> _references = new();

		internal void Add(String key, Object engineRef, Int64 nativeId, Boolean isSceneReference) => _references.Add(key, new EngineReference
		{
			Name = key,
			Value = engineRef,
			NativeId = nativeId,
			IsSceneReference = isSceneReference,
		});

		public Boolean TryGet(String name, out ILunyGameObject obj)
		{
			obj = null;
			if (!_references.TryGetValue(name, out var value))
				return false;

			// TODO: LunyObject should provide a static conversion method, but C# 9 doesn't support static overrides
			// LunyObject has TryGetCached with object registry lookup, this should be utilized
			//
			// could register the object with Luny registry here
			// though this should be avoided in case the object is never actually used
			// but that is only a minor concern/optimization

			//obj = UnityGameObject.ToLunyObject(value);
			return true;
		}
	}
}
