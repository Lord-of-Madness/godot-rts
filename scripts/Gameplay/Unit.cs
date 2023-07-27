using Godot;
using Physics;
using RTSGraphics;
using System;

namespace RTSGameplay
{
    
    
    public partial class Unit : CharacterBody2D,IComparable<Unit>
	{
		public UnitGraphics Graphics;
		public NavigationAgent2D navAgent;


		public Area2D VisionArea;
		[Export(PropertyHint.Range, "0,10,1,or_greater")]
		public float visionRange;//in Tilemeters
		public Tilemeter VisionRange { get => (Tilemeter)visionRange; set { visionRange=(float)value; } }

		/// <summary>
		/// Unit movement speed in Tiles per second
		/// Figure out how to add descriptions in c# (it doesn't appear to be possible)
		/// </summary>
		[Export(PropertyHint.Range, "0,20,1,or_greater")] //doesn't work with non-Variant
        float speed;
		public TilesPerSecond Speed { get=>(TilesPerSecond)speed; set { speed = (float)value; } }

        
		[ExportGroup("CombatStats")]
		[Export] int HP;
		[Export] Godot.Collections.Array<Attack> Attacks;


        public override void _Ready()
		{
			Graphics = GetNode<UnitGraphics>("Graphics");
			navAgent = GetNode<NavigationAgent2D>("NavAgent");
			VisionArea = GetNode<Area2D>("VisionArea");
			((CircleShape2D)VisionArea.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius=VisionRange.ToPixels();

            //navAgent.TargetPosition = Position;//So it doesn't start moving right away
            navAgent.VelocityComputed += GetMoving;
            Deselect();
		}
		/// <summary>
		/// Graphicaly shows the selected status.
		/// </summary>
		public void Select()
		{
			Graphics.Select();
		}
		public void Deselect()
		{
			Graphics.Deselect();
		}
		/// <summary>
		/// Gives the Navigation agent new target.
		/// </summary>
		/// <param name="location"></param>
		public void MoveTo(Vector2 location)
		{
			navAgent.TargetPosition = location;
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
            GD.Print("safe_velocity:");
            GD.Print(safe_velocity.Length());
            MoveAndSlide();
        }

		/// <summary>
		/// Unit comparison based on gameplay priority (sorting player selection etc.)
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
        public int CompareTo(Unit other)
        {
			//Currently ordered by age in scene tree (should be last resort)
			return GetIndex().CompareTo(other.GetIndex());//TODO: Sort units by priority based on their "Heroicness" then the number of abilities, then I guess their cost.
        }
    }
}