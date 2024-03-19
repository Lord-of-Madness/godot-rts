using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using RTS.Gameplay;
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
        /// <summary>
        /// uses string comparison for unequal <c>StringName</c>s (equal use standart StringName comparison)
        /// StringName is meant to be used for equality only. Use with caution
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int CompareTo(this StringName a, StringName b)
        {
            if (a == b) return 0;
            else return ((string)a).CompareTo(b);
        }
        /// <summary>
        /// Destroys Children of the node (RemoveChild followed by QueueFree)
        /// </summary>
        /// <param name="node"></param>
        public static void DestroyChildren(this Node node)
        {
            foreach (var item in node.GetChildren())
            {
                node.RemoveChild(item);
                item.QueueFree();
            }
        }
        public static float LimitWidth(this Camera2D camera)
        {
            return camera.LimitRight - camera.LimitLeft;
        }
        public static float LimitHeight(this Camera2D camera)
        {
            return camera.LimitBottom - camera.LimitTop;
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
