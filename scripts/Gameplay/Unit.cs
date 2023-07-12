using Godot;
using Physics;
using RTSGraphics;
using System;
using static Godot.TextServer;

namespace RTSGameplay
{
	public partial class Unit : CharacterBody2D,IComparable<Unit>
	{
		public bool selected;// this is more of a player to player thing. might not be a part of the units deal
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

		public void Select()
		{
			selected = true;
			Graphics.Select();
		}
		public void Deselect()
		{
			selected = false;
			Graphics.Deselect();
		}
		public void MoveTo(Vector2 location)
		{
			navAgent.TargetPosition = location;
		}
		public override void _PhysicsProcess(double delta)
		{
			if (!navAgent.IsNavigationFinished())//This gets recalculated every physics frame. Not sure if that is necessary (technically it should suffice after each pathcheckpoint (although Graphics certainly need to happen each frame))
			{
				Vector2 direction = Position.DirectionTo(navAgent.GetNextPathPosition());
				navAgent.Velocity = Speed * direction;
                Graphics.MovingTo(direction);
                
			}
			else
				Graphics.Stop();//Perhaps check if moving so this isn't called too much (could add a bool value if it seemed to affect things)
		}
		private void GetMoving(Vector2 safe_velocity)
        {
            Velocity = safe_velocity;
            MoveAndSlide();

        }

        public int CompareTo(Unit other)
        {
			//Currently ordered by age in scene tree (should be last resort)
			return GetIndex().CompareTo(other.GetIndex());//TODO: Sort units by priority based on their "Heroicness" then the number of abilities, then I guess their cost.
        }
    }
}