using Godot;
using Physics;
using RTSGraphics;
namespace RTSGameplay
{
	public partial class Unit : CharacterBody2D
	{
		public bool selected;// this is more of a player to player thing. might not be a part of the units deal
		public UnitGraphics Graphics;
		public NavigationAgent2D navAgent;

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

			//navAgent.TargetPosition = Position;//So it doesn't start moving right away

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
			if (!navAgent.IsNavigationFinished())
			{
				Vector2 direction = Position.DirectionTo(navAgent.GetNextPathPosition());
				Vector2 velocity = Speed * direction;
                navAgent.SetVelocityForced(velocity);
				Graphics.MovingTo(direction);
				Velocity = velocity;
				MoveAndSlide();
			}
			else
				Graphics.Stop();//Perhaps check if moving so this isn't called too much (could add a bool value if it seemed to affect things)
		}
	}
}