using Godot;
using System;

namespace RtsZápočťák
{
    public readonly struct TilesPerSecond
    {
        private readonly float value;
        public TilesPerSecond(float value)
        {
            this.value=value*16;
        }
        public static explicit operator TilesPerSecond(float value)
        {
            return new TilesPerSecond(value);
        }
        public static TilesPerSecond operator *(TilesPerSecond tps,float mult)
        {
            return new TilesPerSecond(tps.value*mult);
        }
        public static Vector2 operator*(TilesPerSecond tps,Vector2 vec)
        {
            return new Vector2(vec*tps.value);
        }
    }
}
