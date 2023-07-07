using Godot;
using System;

public partial class UnitGraphics : Node
{
    private Node2D SelectionVisual;
    private AnimationPlayer anim;
    public static readonly float SQRT2 =(float)Math.Sqrt(2);
    public NavigationAgent2D navAgent;
    private Line2D path;
    private Unit parent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SelectionVisual=GetNode<Node2D>("Selected");
        anim=GetNode<AnimationPlayer>("AnimationPlayer");
        path=GetNode<Line2D>("Path3D");
        GetNode<Sprite2D>("Sprite2D").Frame=1;//This might be removed later
        parent=GetParent<Unit>();
        navAgent=parent.GetNode<NavigationAgent2D>("NavAgent");
    }

    public void Select()
    {
        SelectionVisual.Visible=true;
        path.Visible=true;

    }
    public void Deselect()
    {
        SelectionVisual.Visible=false;
        path.Visible=false;

    }

    internal void MovingTo(Vector2 direction)
    {
        //Sprite:
        if(direction.x>0.5)anim.Play("Right");
        else if(direction.x<-0.5)anim.Play("Left");
        else if(direction.y<-0.5)anim.Play("Back");
        else anim.Play("Forward");
        
        //Path:
        Vector2[] points=navAgent.GetCurrentNavigationPath();
        int from = navAgent.GetCurrentNavigationPathIndex();
        Vector2[] pts= new Vector2[points.Length-from];
        for(int i = 0;i<pts.Length;i++)pts[i]=points[i+from];
        path.GlobalPosition=Vector2.Zero;//So path doesn't move with player
        pts[0]=parent.GlobalPosition;//We don't want to keep drawing the location behind us
        path.Points=pts;

    }
    public void Stop()
    {
        anim.Play("Idle");
    }

}
