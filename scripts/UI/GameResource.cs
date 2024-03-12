using Godot;
using RTS.Gameplay;

namespace RTS.UI
{
    [Tool]
    public partial class GameResource : HBoxContainer//TODO: Split UI from the logic? AI players don't need HBoxContainers
    {
        TextureRect Icon = new();
        [Export]
        Texture2D Texture
        {
            get => Icon.Texture; set => Icon.Texture = value;
        }
        Label ValueLabel=new();
        public static GameResource operator +(GameResource lhs, int rhs) { lhs.Value += rhs; return lhs; }
        public static GameResource operator -(GameResource lhs, int rhs) { lhs.Value -= rhs; return lhs; }
        public static GameResource operator +(GameResource lhs, GameResource rhs) { lhs.Value += rhs.Value; return lhs; }
        public static GameResource operator -(GameResource lhs, GameResource rhs) { lhs.Value -= rhs.Value; return lhs; }


        int val;
        [Export(PropertyHint.Range, "0,1000,10,or_greater")]
        public int Value {
            get => val;
            set {
                val = value;
                ValueLabel.Text= val.ToString();
            }
        }
        public override void _Ready()
        {
            base._Ready();
            Icon = GetNode<TextureRect>(nameof(Icon));
            ValueLabel = GetNode<Label>(nameof(ValueLabel));
            ValueLabel.TooltipText = Name;
        }

    }
}
