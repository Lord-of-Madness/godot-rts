using Godot;
using RTS.Physics;
using RTS.mainspace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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
        private float speed;
        public TilesPerSecond Speed { get => (TilesPerSecond)speed; set { speed = (float)value; } }

        [Export]
        public Player owner;


        [ExportGroup("CombatStats")]
        [Export] public int MaxHP { get; set; }
        private int hp;
        public int HP { get { return hp; } set { hp = value; HealthChanged(); } }
        private ProgressBar HealthBar;
        private Godot.Collections.Array<Attack> Attacks;
        //this was supposed to be done from the inspector but the Attacks weren't unique (It kept interacting with just the last attack on screen so now its a special node in the scene tree under which the attacks are.)
        //private Attack[] Attacks;
        [Export] private int PrimaryAttack;//Changeable by the player. Will change to which position will the unit try to get if it has more than 1 attack available.
        //Could write a Tool script to make this smoother (at the moment I cannot know how many attacks exist when setting PrimaryAttack and therefore cannot add bounds)


        //This will be action queue later now it shall be just one command.
        public UnitAction currentAction;

        private Selectable target;
        private bool following;

        public override void _Ready()
        {
            
            base._Ready();
            HealthBar = Graphics.GetNode<ProgressBar>(nameof(HealthBar));
            HealthBar.MaxValue = MaxHP;
            HP = MaxHP;
            Attacks = new();
            foreach (Attack attack in GetNode<Node>(nameof(Attacks)).GetChildren().Cast<Attack>())
            {
                Attacks.Add(attack);
            }
            if (PrimaryAttack >= Attacks.Count) { PrimaryAttack = 0; }
            navAgent = GetNode<NavigationAgent2D>("NavAgent");
            VisionArea = GetNode<Area2D>("VisionArea");
            ((CircleShape2D)VisionArea.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius = VisionRange.ToPixels();
            navAgent.VelocityComputed += GetMoving;
            navAgent.NavigationFinished += TargetReached;
            navAgent.NavigationFinished += Graphics.NavigationFinished;
            GoIdle();
            Deselect();
            following = false;
        }
        public void CleanCommandQueue()
        {
            Detarget();
        }
        public void Command(Player.ClickMode clickMode, Target target)
        {
            navAgent.AvoidanceEnabled = true;
            //Here should be a check whether mousebutton.Position is a location or a hostile entity.
            switch (clickMode)
            {
                case Player.ClickMode.Move:
                    MoveTo(target);
                    break;
                case Player.ClickMode.Attack:
                    AttackCommand(target);
                    break;
            }
        }
        private void TargetReached()
        {
            if (currentAction == UnitAction.Move) GoIdle();
            Detarget();
        }
        private void GoIdle()
        {
            currentAction = UnitAction.Idle;
            navAgent.AvoidanceEnabled = false;
        }

        public override void _Process(double delta)
        {

            base._Process(delta);
            //if (GetLastSlideCollision() is KinematicCollision2D collision && collision.GetCollider() == target) TargetReached();

            
        }
        /// <summary>
        /// Gives the Navigation agent new target if target is a location.
        /// Otherwise moves to a unit.
        /// </summary>
        /// <param name="target"></param>
        public void MoveTo(Target target)
        {
            currentAction = UnitAction.Move;
            if (target.type == Target.Type.Location) { 
                navAgent.TargetPosition = target.location;
            }
            else
            {
                this.target = target.selectable;
                following = true;
                if (target.selectable is Unit unit)
                {
                    unit.SignalDead += Detarget;
                    //player.VisionArea.BodyExited += Detarget; //TODO when outside vision
                }
            }
        }
        public void AttackCommand(Target target)
        {
            MoveTo(target);//Moving as base plus extra
            currentAction = UnitAction.Attack;
            if (Attacks is not null && Attacks.Count > 0)
            {
                GD.Print("ATTACK!");
                Attacks[PrimaryAttack].AttackAnim(Graphics.Direction);
            }
        }
        private void Detarget()
        {
            GoIdle();
            //navAgent.TargetPosition = Position;
            if (following && target is not null)
            {
                ((Unit)target).SignalDead -= Detarget;
                target = null;
                following = false;
            }
        }
#pragma warning disable IDE1006 // Naming Styles
        public void _on_mouse_entered()
#pragma warning restore IDE1006 // Naming Styles
        {
            owner.JustHovered(this);
        }
#pragma warning disable IDE1006 // Naming Styles
        public void _on_mouse_exited()
#pragma warning restore IDE1006 // Naming Styles
        {
            owner.DeHovered(this);
        }
        public override void _PhysicsProcess(double delta)
        {
            if (following) navAgent.TargetPosition = target.Position;
            if (!navAgent.IsNavigationFinished())
            {
                Vector2 direction = Position.DirectionTo(navAgent.GetNextPathPosition());
                navAgent.Velocity = Speed.GetSpeed() * direction;
                Graphics.MovingTo(direction);
            }
                
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