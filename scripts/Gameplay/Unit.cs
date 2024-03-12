using Godot;
using RTS.Graphics;
using RTS.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.TextEdit;

namespace RTS.Gameplay
{


    public partial class Unit : Damageable, IComparable<Unit>
    {
        /// <summary>
        /// <para>This should Calculate the value of a Unit usefull for ordering and for AI simulations</para>
        /// <para>
        /// It oughta be a combination of: ammount of abilities, Cost, HP, DPS etc.
        /// </para>
        /// </summary>
        public double UnitValue { get; set; }
        /// <summary>
        /// <para>This oughta give us a Value of a freshly recruted unit or in its prime. (freshly recruited units might not start with max Energy/Cooldowns for balancing reasons)</para>
        /// <para>Should be similar to <c>UnitValue</c> but use MAXHP instead of HP etc. </para>
        /// <para>Have to ensure that no two Units have the same <c>BaseUnitValue</c> so that they are always sorted toghther in the UI</para>
        /// <para>On the other hand lets not require that and just sort by UnitName first? ->If so then what if units have the same name...?</para>
        /// </summary>
        public double BaseUnitValue { get; set; }

        /// <summary>
        /// Unit movement speed in Tiles per second
        /// </summary>
        [Export(PropertyHint.Range, "0,20,1,or_greater")] //doesn't work with non-Variant
        private float speed;
        public TilesPerSecond Speed { get => (TilesPerSecond)speed; set { speed = (float)value; } }


        public new UnitGraphics Graphics;
        public NavigationAgent2D NavAgent;
        public Target target;
        private bool following = false;

        public override void _Ready()
        {
            base._Ready();
            Graphics = GetNode<UnitGraphics>(nameof(Graphics));
            NavAgent = GetNode<NavigationAgent2D>(nameof(NavAgent));
            NavAgent.VelocityComputed += GetMoving;
            NavAgent.NavigationFinished += TargetReached;
            NavAgent.NavigationFinished += Graphics.NavigationFinished;
            NavAgent.Radius = ((CircleShape2D)GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius;//So that its always somewhat accurate
            Detarget();
            Deselect();
        }
        public override void CleanCommandQueue()
        {
            base.CleanCommandQueue();
            Detarget();
        }
        public override void Command(Player.ClickMode clickMode, Target target, Ability ability = null)
            //TODO: Consider that Move and Attack are also Abilities. So technicaly they aren't detached from Abilities themselves
        {
            if (CurrentAction == SelectableAction.Dying) return;
            if (target.type == Target.Type.Selectable && target.selectable == this) return;
            //NavAgent.AvoidanceEnabled = true;
            switch (clickMode)
            {
                case Player.ClickMode.Move:
                    MoveTo(target);
                    break;
                case Player.ClickMode.Attack:
                    AttackCommand(target);
                    break;
                case Player.ClickMode.UseAbility:
                    //TODO:CurentAction = UnitAction.?
                    UseAbility(target,ability);
                    break;
            }
        }
        /// <summary>
        /// This gets called by the NavAgent when it reaches its goal
        /// </summary>
        private void TargetReached()
        {
            if (following) return;
            if (CurrentAction == SelectableAction.Move) GoIdle();
        }
        private void GoIdle()
        {
            CurrentAction = SelectableAction.Idle;
            //NavAgent.AvoidanceEnabled = false;
            NavAgent.TargetPosition=Position;
        }
        private double timer = 0;//for debug purposes
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (OS.IsDebugBuild())
            {
                timer += delta;
                if (timer >= 1)
                {
                    //if (CurrentAction != UnitAction.Idle)GD.Print(CurrentAction);
                    timer = 0;
                }
            }
            if (CurrentAction == SelectableAction.Dying) return;
            //if (GetLastSlideCollision() is KinematicCollision2D collision && collision.GetCollider() == target) TargetReached();


        }
        /// <summary>
        /// Gives the Navigation agent new target if target is a location.
        /// Otherwise moves to a unit.
        /// </summary>
        /// <param name="target"></param>
        public void MoveTo(Target target)
        {
            CurrentAction = SelectableAction.Move;
            this.target = target;
            if (target.type == Target.Type.Location)
            {
                NavAgent.TargetPosition = target.location;
            }
            else
            {
                following = true;
                if (target.selectable is Damageable damageable)
                {
                    damageable.SignalDead += Detarget;
                    //player.VisionArea.BodyExited += Detarget; //TODO when outside vision
                }
            }
        }
        public override void TryAgro(Node2D node)
        {
            if(following || CurrentAction == SelectableAction.Move) return;
            if (node is Selectable selectable && selectable.team.IsHostile(team))
            {
                AttackCommand(new Target(selectable));
            }
        }
        public void AttackCommand(Target target)
        {
            //GD.Print("AttackCommand? ",target);
            MoveTo(target);//Moving as base plus extra
            CurrentAction = SelectableAction.Attack;
            RetargetAttacks(target);
        }
        public void UseAbility(Target target,Ability ability)
        {
            MoveTo(target);
            if(ability is TargetedAbility targetedAbility)
            {
                targetedAbility.OnTargetRecieved(target);//TODO: this is clearly kinda wrong and all
            }
        }



        private void Detarget()
        {
            GoIdle();
            DetargetAttacks();
            //navAgent.TargetPosition = Position;
            if (following && target is not null && (target.selectable is Damageable damageable))
            {
                //GD.Print("DETARGETING!");
                damageable.SignalDead -= Detarget;
            }
            target = null;
            following = false;
            
        }

        public override void _PhysicsProcess(double delta)
        {
            if (CurrentAction == SelectableAction.Dying) return;
            if (following && CurrentAction != SelectableAction.Dying) NavAgent.TargetPosition = target.Position;
            if (
                !NavAgent.IsNavigationFinished()
                &&
                Position!= NavAgent.TargetPosition//this is here because when you set NavAgents position to the position of the unit the navigation ain't considered finished
                )
            {
                Vector2 direction = Position.DirectionTo(NavAgent.GetNextPathPosition());
                NavAgent.Velocity = Speed.Speed * direction;
                Graphics.MovingTo(direction);
            }

        }
        /// <summary>
        /// Sets <c>safe_velocity</c> as <c>Velocity</c> and moves the unit along it.
        /// </summary>
        /// <param name="safe_velocity"></param>
        private void GetMoving(Vector2 safe_velocity)
        {
            if (CurrentAction == SelectableAction.Dying) Velocity = Vector2.Zero;
            else Velocity = safe_velocity;
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
            if(this==other) return 0;
            //Sort first by type then by HP
            if(SName!=other.SName) return SName.CompareTo(other.SName);
            if (BaseUnitValue != other.BaseUnitValue) return BaseUnitValue.CompareTo(other.BaseUnitValue);
            if (UnitValue != other.UnitValue) return UnitValue.CompareTo(other.UnitValue);

            if(HP != other.HP) { return HP.CompareTo(other.HP); }//This is a neat sorting to see who needs the least and the most healing (Oughta delete it once we implement UnitValue)

            //Ordered by age in scene tree (should be last resort)
            return GetIndex().CompareTo(other.GetIndex());//TODO: Sort units by priority based on their "Heroicness" then the number of abilities, then I guess their cost.
        }
    }
}