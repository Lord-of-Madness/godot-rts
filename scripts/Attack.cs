using Godot;
using RTS.mainspace;

namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class Attack : Sprite2D
    {
        /*
         Doing it this way allows me to recycle easier however for full release with own models and everythign this should be part of a Unit solidly.
        Also this allows me to edit it in the editor
         */
        public enum DamageType//not gonna use it now (will contain something like: Piercing, Siege etc.)
        {
            Standart
        }
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float AttackSpeed { get; set; }
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float Damage { get; set; }
        [Export] public bool Ranged { get; set; }//If ranged unit will attack once enemy is in range. If melee unit will attack once close and range only affects "escape distance" and/or AoE
        [Export] public bool AoE { get; set; }//Tool needed to require area2D if AoE.
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float Range { get; set; }
        [Export] public DamageType Damagetype { get; set; }
        public AnimationPlayer anim;
        public override void _Ready()
        {
            base._Ready();
            anim = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
        }
        public void AttackAnim(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    anim.Play("Left");
                    break;
                case Direction.Back:
                    anim.Play("Up");
                    break;
                case Direction.Forward:
                    anim.Play("Down");
                    break;
                case Direction.Right:
                    anim.Play("Right");
                    break;
            }
            
        }
        public override string ToString()
        {

            return $"""
                {nameof(AttackSpeed)}:{AttackSpeed}
                {nameof(Damage)}:{Damage}
                {nameof(Damagetype)}:{Damagetype}
                {nameof(Ranged)}:{Ranged}
                {nameof(Range)}:{Range}
                {nameof(AoE)}:{AoE}
                """;

        }
    }
}

