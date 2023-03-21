using Godot;
using System;

public class SelectRect : Node2D
{

    private Vector2 start;
    private Vector2 end;
    private bool dragging;
    public override void _Draw()
    {
        if (dragging)
        {
            DrawRect(new Rect2(start,end-start),new Color(0,1,0,0.3f));
        }
    }
    public void UpdateStats(Vector2 start,Vector2 end,bool dragging)
    {
        this.start=start;
        this.end=end;
        this.dragging=dragging;
        Update();
    }
}
