using Godot;
using System;

public class UnitGraphics : Node
{
    private Node2D SelectionVisual;
    private AnimationPlayer anim;
    public static readonly float SQRT2 =(float)Math.Sqrt(2);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SelectionVisual=GetNode<Node2D>("Selected");
        anim=GetNode<AnimationPlayer>("AnimationPlayer");
        GetNode<Sprite>("Sprite").Frame=1;//This might be removed later
    }

    public void Select()
    {
        SelectionVisual.Visible=true;

    }
    public void Deselect()
    {
        SelectionVisual.Visible=false;

    }

    internal void MovingTo(Vector2 direction)
    {
        //var dot = direction.Dot(Vector2.Left);
        if(direction.x>0.5)anim.Play("Right");
        else if(direction.x<-0.5)anim.Play("Left");
        else if(direction.y<-0.5)anim.Play("Back");
        else anim.Play("Forward");
        
    }
    public void Stop()
    {
        anim.Play("Idle");
    }
}
