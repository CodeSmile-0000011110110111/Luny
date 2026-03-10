using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;

namespace Luny.Unity.Engine.Native
{
	public sealed class PlayerInputProfile
	{
		public UInt32 UserId { get; set; }
		public String UserName { get; set; }
		public Object Actions { get; set; }
		public Object UiInput { get; set; }
		public List<ILunyObject> Pawns { get; } = new();

		public override String ToString() => $"PlayerInputProfile({UserId}:{UserName}, {Actions})";
	}
}
