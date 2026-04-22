using System;
using System.Collections.Generic;

namespace Luny.Engine.Bridge
{
	public sealed class LunyInputUserProfile
	{
		public UInt32 UserId { get; set; }
		public String UserName { get; set; }
		public Object Actions { get; set; }
		public Object UiInput { get; set; }
		public List<LunyGameObject> Pawns { get; } = new();

		public override String ToString() => $"{nameof(LunyInputUserProfile)}({UserId}:{UserName}, {Actions})";
	}
}
