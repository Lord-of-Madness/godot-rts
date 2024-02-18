using Godot;
using System;
using System.Numerics;

namespace RTS.Physics
{
    public static class PhysicsConsts
    {
        public const int TILESIZE = 16;
    }
    public static class PhysicsValues
    {
        public static float Gamespeed { get; set; }
    }
    public record struct TilesPerSecond:
        IMultiplyOperators<TilesPerSecond,float,TilesPerSecond>
    {
        //private static PhysicsValues physicsValues;
        public float value;

        //public static PhysicsValues PhysicsValues { get => physicsValues; set => physicsValues = value; }

        public TilesPerSecond(float value)
        {
            this.value = value * PhysicsConsts.TILESIZE;
        }
        public TilesPerSecond(double value)
        {
            this.value = (float)(value * PhysicsConsts.TILESIZE);
        }
        public static explicit operator TilesPerSecond(float value) => new(value);
        public static explicit operator float(TilesPerSecond value) => value.value;
        public readonly float Speed=> value * PhysicsValues.Gamespeed;
        public static TilesPerSecond operator *(TilesPerSecond tps, float mult) => new(tps.value * mult);
        public static TilesPerSecond operator *(float mult, TilesPerSecond tps) => tps * mult;
        public static Godot.Vector2 operator *(TilesPerSecond tps, Godot.Vector2 direction) => direction * tps.Speed;
        public static Godot.Vector2 operator *(Godot.Vector2 direction, TilesPerSecond tps) => tps * direction;
        public static Tilemeter operator *(TilesPerSecond tps, Second s) => new(tps.value * s.Value);
        public static Tilemeter operator *(Second s, TilesPerSecond tps) => tps * s;
    }
    /// <summary>
    /// A unit of frequency representing number of actions per game second
    /// </summary>
    public readonly record struct Persec
    {
        //public static PhysicsValues physicsValues;
        private readonly float value;
        public Persec(float value)
        {
            this.value = value;
        }
        public float ActionsPerSecond => value * PhysicsValues.Gamespeed;
    }
    public readonly record struct Tilemeter//Tile on its own doesn't sound like a unit of distance.
    {
        public readonly float value;
        public Tilemeter(float value) { this.value = value; }
        public Tilemeter(double value) { this.value = (float)value; }
        public static TilesPerSecond operator /(Tilemeter t, Second s) => new(t.value / s.Value);

        public static explicit operator Tilemeter(float value) => new(value);
        public static explicit operator float(Tilemeter value) => value.value;
        public readonly float Pixels => value * PhysicsConsts.TILESIZE;

    }
    public record struct Second : 
        IComparable<float>,
        IAdditionOperators<Second,float,Second>,
        IAdditionOperators<Second, double, Second>,
        ISubtractionOperators<Second,float,Second>,
        ISubtractionOperators<Second, double, Second>,
        IComparable<Second>
    {
        //private static PhysicsValues physicsValues;
        private readonly double value;
        public readonly double Value { get => value; }
        public Second(float value) { this.value = value; }
        public Second(double value) { this.value = value; }
        public static bool operator ==(Second s1, float s2) => s1.value == s2;
        public static bool operator !=(Second s1, float s2) => s1.value != s2;
        public static bool operator <=(Second s1, float s2) => s1.value <= s2;
        public static bool operator >=(Second s1, float s2) => s1.value >= s2;
        public static bool operator <(Second s1, float s2) => s1.value < s2;
        public static bool operator >(Second s1, float s2) => s1.value > s2;

        public static bool operator <=(Second s1, Second s2) => s1.value <= s2.value;
        public static bool operator >=(Second s1, Second s2) => s1.value >= s2.value;
        public static bool operator <(Second s1, Second s2) => s1.value < s2.value;
        public static bool operator >(Second s1, Second s2) => s1.value > s2.value;


        public static Second operator -(Second s1, float s2) => new(s1.value - s2 * PhysicsValues.Gamespeed);
        public static Second operator +(Second s1, float s2) => new(s1.value + s2 * PhysicsValues.Gamespeed);
        public static Second operator +(Second s1, double s2)=>new(s1.value + s2 * PhysicsValues.Gamespeed);
        public static Second operator -(Second s1, double s2) => new(s1.value - s2 * PhysicsValues.Gamespeed);

        public static implicit operator Second(float v) => new(v);

        public readonly int CompareTo(float other) => value.CompareTo(other);

        public readonly int CompareTo(Second other) => value.CompareTo(other.value);
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
