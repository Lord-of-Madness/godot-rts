using System;
using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class Building : Damageable, IComparable<Building>
    {

        private Target rallyPoint;
        private bool following = false;
        Line2D RallyPath;

        public Shape2D BuildingCollision { get => GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape; }
        public Target RallyPoint
        {
            get => rallyPoint; set
            {
                rallyPoint = value;
                RallyPath.Points = new Vector2[2] { Position, value.Position };//It behaved mighty strange when I tried to just change the second position. The reassignment didn't work (was simply ignored)
            }
        }

        public override void _Ready()
        {
            base._Ready();
            RallyPath = Graphics.GetNode<Line2D>("PathLine");
            RallyPoint = new(this);
            RallyPath.GlobalPosition = Vector2.Zero;//So path doesn't move with the unit
        }

        public override void CleanCommandQueue()
        {
            return;//Buildings don't have commmand queues
            //TODO: Ponder moving CleanCommandQueue only to Unit away from Selectables
        }

        public override void Command(Player.ClickMode clickMode, Target target, Ability ability = null)
        {
            switch (clickMode)
            {
                case Player.ClickMode.Move:
                    SetRally(target);
                    break;
            }
        }
        public void SetRally(Target target)
        {
            RallyPoint = target;
            if (target.type == Target.Type.Selectable)
            {
                following = true;
                if (target.selectable is Damageable damageable)//TODO: The damageable should perhaps be Selectable and it should deRally even on disapearing into the fog of war if its not our Selectable
                {
                    damageable.SignalDead += () => SetRally(new(damageable.Position));//This oughta mean that the Rally point stays where the unit died
                    //player.VisionArea.BodyExited += Detarget; //TODO when outside vision
                }
            }
        }
        public override void _PhysicsProcess(double delta)
        {
            //if (CurrentAction == UnitAction.Dying) return;
#pragma warning disable CA2245 // Do not assign a property to itself
            if (following) RallyPoint = RallyPoint;//I know its kinda ugly but it works!
#pragma warning restore CA2245 // Do not assign a property to itself
        }
        public int CompareTo(Building other)
        {
            return GetIndex().CompareTo(other.GetIndex());//For now
        }

    }
}
