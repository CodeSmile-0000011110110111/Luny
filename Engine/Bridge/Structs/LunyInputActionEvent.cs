using System;

namespace Luny.Engine.Bridge
{
	public sealed class LunyInputActionEvent
	{
		public String ActionMapName;
		public String ActionName;
		public String UserName;
		public LunyInputActionPhase Phase;
	}

	public enum LunyInputActionPhase
	{
		Disabled,
		Waiting,
		Started,
		Performed,
		Canceled,
		Performing,
	}
}
