namespace Luny.Engine.Bridge
{
	public enum LunyAxis
	{
		X,
		Y,
		Z,
	}

	public static class LunyAxisExtension
	{
		public static LunyVector3 ToVector3(this LunyAxis axis) => axis switch
		{
			LunyAxis.X => LunyVector3.Right,
			LunyAxis.Y => LunyVector3.Up,
			var _ => LunyVector3.Forward,
		};
	}
}
