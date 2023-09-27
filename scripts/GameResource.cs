using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoCustomResourceRegistry;

namespace RTS.Gameplay
{
    [RegisteredType(nameof(GameResource))]
    public partial class GameResource : Resource
    {
        public static GameResource operator +(GameResource lhs, GameResource rhs) => new(lhs.Value + rhs.Value);
        public static GameResource operator -(GameResource lhs, GameResource rhs) => new(lhs.Value - rhs.Value);

        [Export]
        public Texture2D icon;
        [Export]
        public string name;

        public float Value { get; set; }

        public GameResource(float Value)
        {
            this.Value = Value;
        }
        public GameResource()
        {

        }
    }
}
