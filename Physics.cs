using Godot;

namespace Physics
{
    public readonly record struct TilesPerSecond
    {
        private readonly float value;
        public TilesPerSecond(float value)
        {
            this.value=value*16;
        }
        public static explicit operator TilesPerSecond(float value) => new(value);
        public static explicit operator float(TilesPerSecond value) => value.value;
        public static TilesPerSecond operator *(TilesPerSecond tps, float mult) => new(tps.value * mult);
        public static TilesPerSecond operator *(float mult, TilesPerSecond tps) => tps * mult;
        public static Vector2 operator *(TilesPerSecond tps, Vector2 vec) => vec * tps.value;
        public static Vector2 operator *(Vector2 vec, TilesPerSecond tps) => vec * tps;



    }
    public record struct Tilemeter//Tile on its own doesn't sound like a unit of distance.
    {
        public float value;
        public Tilemeter(float value) { this.value = value; }
        public static TilesPerSecond operator /(Tilemeter t, Second s)
        {
            return (TilesPerSecond)(t.value/s.value);
        }


    }
    public record struct Second
    {
        public float value;
        public Second(float value) { this.value = value;}
    }
    public static class PhysicsExtensions
    {
        /// <summary>
        /// Converts units into Tiles -> divides by 16
        /// (casting into Tilemeters doesn't divide the value)
        /// </summary>
        public static Tilemeter ToTilemeter(this float f)
        {
            return new Tilemeter(f/16);
        }
    }

}
