using Godot;
using RtsZápočťák;
using System;

public partial class Unit : CharacterBody2D
{
	public bool selected;
	public UnitGraphics Graphics;
	public NavigationAgent2D navAgent;

	[Export] //doesn't work with non-Variant
	float tps;
	public TilesPerSecond Speed{ get;set;}


	public override void _Ready()
	{
		Graphics=GetNode<UnitGraphics>("Graphics");
		navAgent=GetNode<NavigationAgent2D>("NavAgent");
		Speed = new TilesPerSecond(tps);

		navAgent.TargetPosition = Position;//So it doesn't start moving right away
		Deselect();
	}

	public void Select()
	{
		selected=true;
		Graphics.Select();
	}
	public void Deselect()
	{
		selected=false;
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
			Vector2 velocity = Speed*direction;
			navAgent.SetVelocity(velocity);
			Graphics.MovingTo(direction);
			Velocity = velocity;
			MoveAndSlide();
		}
		else
			Graphics.Stop();//Perhaps check if moving so this isn't called too much

	}
}
