using Godot;
using Godot.Collections;
using System.Collections.Generic;

namespace RTS.mainspace
{
    using GVector2 = Godot.Vector2;
    public static class GodotExtensions
    {
        public static Array<T> ToGodotArray<[MustBeVariant] T>(this IEnumerable<T> e)
        {
            Array<T> array = new();
            var en = e.GetEnumerator();
            while (en.MoveNext())
            {
                array.Add(en.Current);
            }
            return array;
        }
    }
    public enum Direction
    {
        Back,
        Forward,
        Left,
        Right
    }
    
}
