using Godot;
using RTS.mainspace;
using RTS.Physics;

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
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float AttackSpeed { get; set; }//Attacks per second
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float Damage { get; set; }
        [Export] public bool Ranged { get; set; }//If ranged unit will attack once enemy is in range. If melee unit will attack once close and range only affects "escape distance" and/or AoE
        [Export] public bool AoE { get; set; }//Tool needed to require area2D if AoE.
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float range;//in tiles
        public Tilemeter Range { get { return (Tilemeter)range; } set { range = (float)value; } }
        [Export] public DamageType Damagetype { get; set; }
        public AnimationPlayer anim;
        public Area2D AttackRange;
        public Target target;
        public bool targetInRange = false;
        public double cooldown;
        public double AttackPeriod { get { return 1 / AttackSpeed; } }
        public Unit owner;
        public override void _Ready()
        {
            base._Ready();
            anim = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
            AttackRange = GetNode<Area2D>(nameof(AttackRange));
            ((CircleShape2D)AttackRange.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius = Range.ToPixels();
            cooldown = AttackPeriod;
            AttackRange.BodyEntered += TargetEnteredRange;
            AttackRange.BodyExited += TargetLeftRange;
        }
        public void AttackAnim(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    anim.Play(Direction.Left.ToString());
                    break;
                case Direction.Back:
                    anim.Play(Direction.Back.ToString());
                    break;
                case Direction.Forward:
                    anim.Play(Direction.Forward.ToString());
                    break;
                case Direction.Right:
                    anim.Play(Direction.Right.ToString());
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
                {nameof(Range)}:{Range} Tiles
                {nameof(AoE)}:{AoE}
                """;

        }
        public void TargetEnteredRange(Node2D inrangee)
        {
            if (target is null || owner.CurrentAction != Unit.UnitAction.Attack) return;

            GD.Print(inrangee, " entered");
            if (inrangee is Selectable selectable
                && selectable == target.selectable)
            {
                targetInRange = true;
                GD.Print("Target in range!");
            }
        }
        public void TargetLeftRange(Node2D body)
        {
            if (target is null) return;
            GD.Print(body, " left");
            if (target.type == Target.Type.Selectable && body is Selectable selectable
            && selectable == target.selectable)
            {
                targetInRange = false;
            }
        }
        public void Retarget(Target target)
        {
            this.target = target;
            if(target.type==Target.Type.Selectable && AttackRange.GetOverlappingBodies().Contains(target.selectable))
                targetInRange = true;
            else targetInRange = false;
        }
    }
}

