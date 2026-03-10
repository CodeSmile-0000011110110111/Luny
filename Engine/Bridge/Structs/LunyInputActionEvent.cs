using System;

namespace Luny.Engine.Bridge
{
	public sealed class LunyInputActionEvent
	{
		public String ActionMapName;
		public String ActionName;
		public String UserName;
		public Int32 DeviceId;
		public LunyInputActionPhase Phase;

		public override String ToString() => $"Input({UserName}, {ActionMapName}/{ActionName}, {Phase})";
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
