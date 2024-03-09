using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System.Collections.Generic;

namespace RTS.mainspace
{
    using GVector2 = Godot.Vector2;
    public static class GodotExtensions
    {
        /// <summary>
        /// Converts Enumerable into a Godot.Array
        /// </summary>
        /// <typeparam name="TVariant"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static Array<TVariant> ToGodotArray<[MustBeVariant] TVariant>(this IEnumerable<TVariant> enumerable)
        {
            
            Array<TVariant> array = new();
            var en = enumerable.GetEnumerator();
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
