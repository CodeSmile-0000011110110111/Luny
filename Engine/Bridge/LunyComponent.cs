namespace Luny.Engine.Bridge
{
	public interface ILunyComponent : ILunyObject {}
	public abstract class LunyComponent : LunyObject, ILunyComponent {}
}
