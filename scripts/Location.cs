using Godot;


namespace RTS.Gameplay
{
    public struct Location : ITargetable
    {
        private Vector2 data;
        public readonly Vector2 Position => data;
        public Location(Vector2 v)
        {
            data = v;
        }
        public static implicit operator Vector2(Location location) => location.data;
        public static implicit operator Location(Vector2 v) => new(v);
        public override readonly string ToString() =>data.ToString();

    }
}