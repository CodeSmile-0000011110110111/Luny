using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace Luny.Unity.Engine.Native
{
	public sealed class PlayerInputProfile
	{
		public UInt32 UserId { get; init; }
		public String UserName { get; set; }
		public Object ActionAsset { get; set; }
		public Object UiInput { get; init; }
		public List<ILunyObject> Pawns { get; } = new();
		public Boolean IsHost => UserId == 0;

		public override String ToString() => $"PlayerInputProfile({UserId}, {ActionAsset})";
	}
}
