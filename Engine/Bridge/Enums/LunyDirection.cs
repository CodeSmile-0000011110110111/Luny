namespace Luny.Engine.Bridge
{
	public enum LunyAxisDirection
	{
		Forward,
		Back,
		Right,
		Left,
		Up,
		Down,
	}

	public enum LunyCardinalDirection
	{
		// Horizontal Plane (3D Forward/Back)
		North = 0,
		East = 2,
		South = 4,
		West = 6,

		// Vertical Plane
		Up = 8,
		Down = 9,
	}

	public enum LunyOrdinalDirection
	{
		NorthEast = 1,
		SouthEast = 3,
		SouthWest = 5,
		NorthWest = 7,
	}

	public enum LunyDirection8
	{
		North = LunyCardinalDirection.North,
		NorthEast = LunyOrdinalDirection.NorthEast,
		East = LunyCardinalDirection.East,
		SouthEast = LunyOrdinalDirection.SouthEast,
		South = LunyCardinalDirection.South,
		SouthWest = LunyOrdinalDirection.SouthWest,
		West = LunyCardinalDirection.West,
		NorthWest = LunyOrdinalDirection.NorthWest,
	}
}
