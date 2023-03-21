using Godot;
using RtsZápočťák;
using System;

public class Unit : KinematicBody2D
{
    public bool selected;
    public UnitGraphics Graphics;
    public NavigationAgent2D navAgent;

    //[Export] doesn't work with non-Variant
    public TilesPerSecond speed{ get;set;}=new TilesPerSecond(5);


    public override void _Ready()
    {
        Graphics=GetNode<UnitGraphics>("Graphics");
        navAgent=GetNode<NavigationAgent2D>("NavAgent");


        navAgent.SetTargetLocation(Position);//So it doesn't start moving right away
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
        navAgent.SetTargetLocation(location);
    }
    public override void _PhysicsProcess(float delta)
    {
        //base._PhysicsProcess(delta);//Don't think this is needed
        
        if (!navAgent.IsNavigationFinished())
        {
            Vector2 direction = Position.DirectionTo(navAgent.GetNextLocation());
            Vector2 velocity = speed*direction;
            navAgent.SetVelocity(velocity);
            Graphics.MovingTo(direction);
            MoveAndSlide(velocity);
        }
        else
            Graphics.Stop();//Perhaps check if moving so this isn't called too much

    }
}
