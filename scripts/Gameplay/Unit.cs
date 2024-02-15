using Godot;
using RTS.Graphics;
using RTS.Physics;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// </summary>
        public double BaseUnitValue { get; set; }

        


        /// <summary>
        /// Unit movement speed in Tiles per second
        /// Figure out how to add descriptions in c# (it doesn't appear to be possible)
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
            Attacks = GetNode<AttacksNode>(nameof(Attacks)).Attacks;
            NavAgent = GetNode<NavigationAgent2D>(nameof(NavAgent));
            NavAgent.VelocityComputed += GetMoving;
            NavAgent.NavigationFinished += TargetReached;
            NavAgent.NavigationFinished += Graphics.NavigationFinished;
            NavAgent.Radius = ((CircleShape2D)GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius + 1;//So that its always somewhat accurate
            GoIdle();
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
                    CurrentAction = SelectableAction.Move;
                    MoveTo(target);
                    break;
                case Player.ClickMode.Attack:
                    CurrentAction = SelectableAction.Attack;
                    AttackCommand(target);
                    break;
                case Player.ClickMode.UseAbility:
                    //TODO:CurentAction = UnitAction.?
                    UseAbility(target,ability);
                    break;
            }
        }
        private void TargetReached()
        {
            if (following) return;
            if (CurrentAction == SelectableAction.Move) GoIdle();
            Detarget();
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
            foreach (Attack attack in Attacks)
            {
                if (attack.cooldown < attack.AttackPeriod) attack.cooldown += delta;
                if (attack.targetInRange)
                {
                    if (attack.cooldown >= attack.AttackPeriod)
                    {
                        attack.AttackAnim(Graphics.Direction);
                        attack.cooldown = 0;
                        Damageable damagableTarget = (Damageable)attack.target.selectable;
                        damagableTarget.HP = (int)Math.Round(damagableTarget.HP - attack.Damage);//Not the nicest but feels the correctest
                    }
                }
            }
            //if (GetLastSlideCollision() is KinematicCollision2D collision && collision.GetCollider() == target) TargetReached();


        }
        /// <summary>
        /// Gives the Navigation agent new target if target is a location.
        /// Otherwise moves to a unit.
        /// </summary>
        /// <param name="target"></param>
        public void MoveTo(Target target)
        {
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
        public void AttackCommand(Target target)
        {
            //GD.Print("AttackCommand? ",target);
            MoveTo(target);//Moving as base plus extra
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

        private void RetargetAttacks(Target target)
        {
            if (Attacks is not null)
            {
                foreach (var attack in Attacks)
                {
                    attack.Retarget(target);
                    /*
                    attack.AttackRange.BodyEntered += (body) =>
                    {
                        AttackTargetInRange(body, attack);
                    };
                    //attack.AttackRange.BodyEntered += (Node2D smthn) => { GD.Print("Whatever"); };
                    //attack.AttackRange.AreaEntered += (Area2D smthn) => { GD.Print(smthn); };
                    //GD.Print(((CircleShape2D)attack.AttackRange.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius);
                    */
                }

            }
        }
        private void DetargetAttacks()
        {
            if (Attacks is not null)
            {
                foreach (var attack in Attacks)
                {
                    attack.Detarget();
                }

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
            target = new();
            following = false;
            
        }

        public override void _PhysicsProcess(double delta)
        {
            //if (CurrentAction == UnitAction.Dying) return;
            if (following) NavAgent.TargetPosition = target.Position;
            if (!NavAgent.IsNavigationFinished())
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
            if (BaseUnitValue != other.BaseUnitValue) return BaseUnitValue.CompareTo(other.BaseUnitValue);
            if (UnitValue != other.UnitValue) return UnitValue.CompareTo(other.UnitValue);

            if(HP != other.HP) { return HP.CompareTo(other.HP); }//This is a neat sorting to see who needs the least and the most healing (Oughta delete it once we implement UnitValue)

            //Ordered by age in scene tree (should be last resort)
            return GetIndex().CompareTo(other.GetIndex());//TODO: Sort units by priority based on their "Heroicness" then the number of abilities, then I guess their cost.
        }

        public override void Dead()
        {
            //GD.Print("Is this the end?");
            CurrentAction = SelectableAction.Dying;
            EmitSignal(SignalName.SignalDisablingSelection,this);
            EmitSignal(SignalName.SignalDead);
            CleanCommandQueue();
            //leave corpse?
            Graphics.DeathAnim();//At the end it will remove the unit

        }
    }
}