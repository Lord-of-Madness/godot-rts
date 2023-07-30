using Godot;
using RTS.Physics;
using RTS.mainspace;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RTS.Gameplay
{


    public partial class Unit : Selectable, IComparable<Unit>, IDamagable
    {
        [Signal] public delegate void SignalDeadEventHandler();
        [Signal] public delegate void SignalDamagedEventHandler();
        [Signal] public delegate void SignalHealthChangedEventHandler();

        public enum UnitAction
        {
            Move,
            Attack,
            Idle,
            Stay,
            Patrol
        }
        public NavigationAgent2D navAgent;

        public Area2D VisionArea;
        [Export(PropertyHint.Range, "0,10,1,or_greater")]
        public float visionRange;//in Tilemeters
        public Tilemeter VisionRange { get => (Tilemeter)visionRange; set { visionRange = (float)value; } }

        /// <summary>
        /// Unit movement speed in Tiles per second
        /// Figure out how to add descriptions in c# (it doesn't appear to be possible)
        /// </summary>
        [Export(PropertyHint.Range, "0,20,1,or_greater")] //doesn't work with non-Variant
        float speed;
        public TilesPerSecond Speed { get => (TilesPerSecond)speed; set { speed = (float)value; } }

        [Export]
        public Player owner;


        [ExportGroup("CombatStats")]
        [Export] public int MaxHP { get; set; }
        public int HP { get; set; }
        private ProgressBar HealthBar;
        private Godot.Collections.Array<Attack> Attacks;
        //private Attack[] Attacks;
        [Export] private int PrimaryAttack;//Changeable by the player. Will change to which position will the unit try to get if it has more than 1 attack available.
        //Could write a Tool script to make this smoother (at the moment I cannot know how many attacks exist when setting PrimaryAttack and therefore cannot add bounds)


        //This will be action queue later now it shall be just one command.
        public UnitAction currentAction;

        private Selectable target;

        public override void _Ready()
        {
            currentAction = UnitAction.Idle;
            base._Ready();
            HealthBar = Graphics.GetNode<ProgressBar>(nameof(HealthBar));
            HealthBar.MaxValue = MaxHP;
            HP = MaxHP;
            Attacks = new();
            foreach (Node attack in GetNode<Node>(nameof(Attacks)).GetChildren())
            {
                Attacks.Add((Attack)attack);
            }
            if(PrimaryAttack>=Attacks.Count) { PrimaryAttack = 0; }
            navAgent = GetNode<NavigationAgent2D>("NavAgent");
            VisionArea = GetNode<Area2D>("VisionArea");
            ((CircleShape2D)VisionArea.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius = VisionRange.ToPixels();
            navAgent.VelocityComputed += GetMoving;
            navAgent.TargetReached += TargetReached;
            Deselect();
        }

        private void TargetReached()
        {
            if(currentAction == UnitAction.Move)currentAction= UnitAction.Idle;
        }
        private void Follow(Selectable target)
        {
            this.target = target;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            
        }
        /// <summary>
        /// Gives the Navigation agent new target if target is a location.
        /// Otherwise moves to a unit.
        /// </summary>
        /// <param name="target"></param>
        public void MoveTo(Target target)
        {
            currentAction = UnitAction.Move;
            if(target.type==Target.Type.Location)
                navAgent.TargetPosition = target.location;
            else
            {
                this.target = target.selectable;
            }
        }
        public void AttackCommand(Target target)
        {
            MoveTo(target);//Moving as base plus extra
            currentAction = UnitAction.Attack;
            if(target.type == Target.Type.Selectable && target.selectable is Unit unit)
            {
                unit.SignalDead += Detarget;

            }
            if (Attacks is not null && Attacks.Count > 0)
            {
                Attacks[PrimaryAttack].AttackAnim(Graphics.Direction);
            }
        }
        private void Detarget()
        {
            currentAction= UnitAction.Idle;
            navAgent.TargetPosition = Position;
            target = null;
        }
        public void _on_mouse_entered()
        {
            owner.JustHovered(this);
        }
        public void _on_mouse_exited()
        {
            owner.DeHovered(this);
        }
        public override void _PhysicsProcess(double delta)
        {
            if (!navAgent.IsNavigationFinished())//This gets recalculated every physics frame. Not sure if that is necessary (technically it should suffice after each pathcheckpoint (although Graphics certainly need to happen each frame))
            {
                Vector2 direction = Position.DirectionTo(navAgent.GetNextPathPosition());
                navAgent.Velocity = Speed.GetSpeed() * direction;
                Graphics.MovingTo(direction);

            }
            else
                Graphics.Stop();//Perhaps check if moving so this isn't called too much (could add a bool value if it seemed to affect things)
        }
        /// <summary>
        /// Sets <c>safe_velocity</c> as <c>Velocity</c> and moves the unit along it.
        /// </summary>
        /// <param name="safe_velocity"></param>
        private void GetMoving(Vector2 safe_velocity)
        {
            Velocity = safe_velocity;
            MoveAndSlide();
        }

        /// <summary>
        /// Unit comparison based on gameplay priority (sorting player selection etc.)
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// less than zero => lesser<br />
		/// equal to 0 => equal<br />
		/// greater than 0 => greater<br />
        /// </returns>
        public int CompareTo(Unit other)
        {
            //Currently ordered by age in scene tree (should be last resort)
            return GetIndex().CompareTo(other.GetIndex());//TODO: Sort units by priority based on their "Heroicness" then the number of abilities, then I guess their cost.
        }

        public void HealthChanged()
        {
            HealthBar.Value = HP;
            GD.PrintErr(EmitSignal(nameof(SignalHealthChanged)));
            if (HP <= 0) Dead();
        }

        public void Dead()
        {
            EmitSignal(nameof(SignalDead));
        }

        public void Damaged()
        {
            HealthChanged();
            EmitSignal(nameof(SignalDamaged));
        }
    }
}