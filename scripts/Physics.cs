using Godot;

namespace Physics
{
    public static class PhysicsConsts
    {
        public const int TILESIZE = 16;
    }
    public class PhysicsValues
    {
        public float Gamespeed { get; set; }
    }
    public record struct TilesPerSecond
    {
        public static PhysicsValues physicsValues;
        public float value;
        public TilesPerSecond(float value)
        {
            this.value = value * PhysicsConsts.TILESIZE;
        }
        public static explicit operator TilesPerSecond(float value) => new(value);
        public static explicit operator float(TilesPerSecond value) => value.value;
        public float GetSpeed() => value * physicsValues.Gamespeed;
        public static TilesPerSecond operator *(TilesPerSecond tps, float mult) => new(tps.value * mult);
        public static TilesPerSecond operator *(float mult, TilesPerSecond tps) => tps * mult;
        public static Vector2 operator *(TilesPerSecond tps, Vector2 direction) => direction * tps.GetSpeed();
        public static Vector2 operator *(Vector2 direction, TilesPerSecond tps) => direction * tps;
        public static Tilemeter operator *(TilesPerSecond tps, Second s) => new(tps.value * s.value);
        public static Tilemeter operator *(Second s, TilesPerSecond tps) => tps * s;
    }
    /// <summary>
    /// A unit of frequency representing number of actions per game second
    /// </summary>
    public readonly record struct Persec
    {
        public static PhysicsValues physicsValues;
        private readonly float value;
        public Persec(float value)
        {
            this.value = value;
        }
        public float GetActionsPerSecond() =>value*physicsValues.Gamespeed;
    }
    public record struct Tilemeter//Tile on its own doesn't sound like a unit of distance.
    {
        public float value;
        public Tilemeter(float value) { this.value = value; }
        public static TilesPerSecond operator /(Tilemeter t, Second s) => new(t.value / s.value);

        public static explicit operator Tilemeter(float value) => new(value);
        public static explicit operator float(Tilemeter value) => value.value;
        public float ToPixels() => value * PhysicsConsts.TILESIZE;

    }
    public record struct Second
    {
        public float value;
        public Second(float value) { this.value = value; }
    }
    public static class PhysicsExtensions
    {
        /// <summary>
        /// Converts units into Tiles -> divides by <c>PhysicsConsts.TILESIZE</c> (at the time of writing thats 16)
        /// (casting into Tilemeters doesn't divide the value)
        /// </summary>
        public static Tilemeter FromPixelsToTilemeter(this float pixels) => new(pixels / PhysicsConsts.TILESIZE);
    }

}
