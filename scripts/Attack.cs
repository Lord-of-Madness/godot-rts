using Godot;
using RTS.mainspace;
using RTS.Physics;
using System;

namespace RTS.Gameplay
{
    /// <summary>
    /// Class that gets attached in the editor to given Selectable under the Attacks node
    /// </summary>
    /// <remarks>
    /// Has general 
    /// </remarks>
    [GlobalClass]
    public partial class Attack : Node2D
    {
        /// <summary>
        /// Unused for now (TODO: contain something like: Piercing, Siege etc.)
        /// </summary>
        public enum DamageType
        {
            Standart
        }
        /// <summary>
        /// Attacks per second a.k.a frequency
        /// </summary>
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float AttackSpeed { get; set; }
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float Damage { get; set; }
        [Export] public bool Ranged { get; set; }//If ranged unit will attack once enemy is in range. If melee unit will attack once close and range only affects "escape distance" and/or AoE
        [Export] public bool AoE { get; set; }//Tool needed to require area2D if AoE.


        [Export] public DamageType Damagetype { get; set; }
        public AnimationPlayer anim;
        public Area2D AttackRange;
        public Target target;
        public bool targetInRange = false;
        /// <summary>
        /// current time before next attack (set to Attack period going down to 0)
        /// </summary>
        public Second cooldown;
        public Second AttackPeriod { get { return 1 / AttackSpeed; } }
        /// <summary>
        /// range in tiles. To be exportable must be float. Will get reinterpreted as Tilemeters elsewhere in the code
        /// </summary>
        [Export(PropertyHint.Range, "0,10,1,or_greater")]
        public float range;
        /// <summary>
        /// Attack range
        /// </summary>
        public Tilemeter Range
        {
            get => (Tilemeter)range;
            set => range = (float)value;
        }

        /*public float RangeExport//This exists simply as means to export the float and set Range proper
        {
            get => range; set
            {
                range =value;
                GD.Print(Range.Pixels);
                ((CircleShape2D)GetNode<Area2D>(nameof(AttackRange)).GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius = Range.Pixels;
            }
        }*/

        public Selectable owner;
        public Sprite2D Graphic;
        public override void _Ready()
        {
            base._Ready();
            owner = GetParent().GetParent<Selectable>();
            Graphic = GetNode<Sprite2D>(nameof(Graphic));
            anim = Graphic.GetNode<AnimationPlayer>(nameof(AnimationPlayer));
            AttackRange = GetNode<Area2D>(nameof(AttackRange));
            ((CircleShape2D)AttackRange.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius = Range.Pixels;
            cooldown = AttackPeriod;
            AttackRange.BodyEntered += TargetEnteredRange;
            AttackRange.BodyExited += TargetLeftRange;
        }
        public override void _Process(double delta)
        {

            base._Process(delta);
            if (cooldown < AttackPeriod) cooldown += delta;
            if (targetInRange)
            {
                if (cooldown >= AttackPeriod)
                {

                    AttackAnim(owner.Graphics.Direction);
                    cooldown = 0;
                    Damageable damagableTarget = (Damageable)target.selectable;
                    damagableTarget.HP = (int)Math.Round(damagableTarget.HP - Damage);//Not the nicest but feels the correctest
                }
            }
        }
        /// <summary>
        /// Plays attack animation in the specified direction
        /// </summary>
        /// <param name="direction"></param>
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
            if (target is null || owner.CurrentAction != Selectable.SelectableAction.Attack) return;

            GD.Print(inrangee.Name, " entered and has type: ", inrangee.GetType());
            if (inrangee is Selectable selectable
                && selectable == target.selectable)
            {
                targetInRange = true;
                //GD.Print("Target in range!");
            }
        }
        public void TargetLeftRange(Node2D body)
        {
            if (target is null) return;
            //GD.Print(body.Name, " left");
            if (target.type == Target.Type.Selectable && body is Selectable selectable
            && selectable == target.selectable)
            {
                targetInRange = false;
            }
        }
        /// <summary>
        /// Changes target for a new one
        /// Also checks if it is in range already
        /// </summary>
        /// <param name="target">new target</param>
        public void Retarget(Target target)
        {
            this.target = target;
            //GD.Print("Retargetin! "+target);
            if (target.type == Target.Type.Selectable && AttackRange.GetOverlappingBodies().Contains(target.selectable))
                targetInRange = true;
            else targetInRange = false;
        }
        /// <summary>
        /// Nulls out <c>target</c>
        /// </summary>
        public void Detarget()
        {
            //GD.Print("Detarget");
            target = null;
            targetInRange = false;
        }
    }
}

