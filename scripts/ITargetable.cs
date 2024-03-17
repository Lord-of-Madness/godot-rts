using Godot;


namespace RTS.Gameplay
{
    public interface ITargetable//Interface might be better than a class ->TODO conisder
    {
        public Vector2 Position { get; }
        public string ToString();
        //[Signal] public delegate void SignalBeingDisposedOfEventHandler(ITargetable targetable); Waiting for pull request to happen
    }
}