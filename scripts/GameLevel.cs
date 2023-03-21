using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

using GDArray = Godot.Collections.Array;

public class GameLevel : Node
{


    // Called when the node enters the scene tree for the first time.
    private GDArray selectedUnits;
    private RectangleShape2D selectRect;
    private SelectRect selectRectNode;
    private bool dragging;
    private Vector2 dragStart;
    private Node2D localLevel;
    public override void _Ready()
    {
        selectRect = new RectangleShape2D();
        selectRectNode = GetNode<SelectRect>("SelectRect");
        dragging = false;
        localLevel = GetParent<Node2D>();
        selectedUnits = new GDArray();
        
    }
    public void TogglePause(bool toggleOn)
    {
        localLevel.GetTree().Paused=toggleOn;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (@event is InputEventMouseButton mousebutton) 
            if (mousebutton.ButtonIndex == (int)ButtonList.Left)
            {
                if (mousebutton.Pressed)
                {
                    foreach (object shape in selectedUnits)
                    {
                        if (shape is Unit unit)
                        {
                            unit.Deselect();
                        }
                    }
                    selectedUnits = new GDArray();
                    dragging = true;
                    dragStart = mousebutton.Position;
                }
                else if (dragging)
                {
                    dragging = false;
                    var dragEnd = mousebutton.Position;
                    selectRectNode.UpdateStats(dragStart, dragEnd, dragging);
                    selectRect.Extents = (dragEnd - dragStart) / 2;

                    var spaceState = localLevel.GetWorld2d().DirectSpaceState;
                    var query = new Physics2DShapeQueryParameters();
                    query.SetShape(selectRect);
                    query.Transform = new Transform2D(0, (dragEnd + dragStart) / 2);
                    var selected = spaceState.IntersectShape(query);
                    foreach (Dictionary shape in selected)
                    {
                        if (shape["collider"] is Unit unit)
                        {
                            unit.Select();
                            selectedUnits.Add(unit);
                        }

                    }
                }
            }
        else if (mousebutton.ButtonIndex == (int)ButtonList.Right)
            {
                foreach (Unit unit in selectedUnits)
                {
                    unit.MoveTo(mousebutton.Position);
                }
            }
        if (dragging && @event is InputEventMouseMotion mousemotion)
        {
            selectRectNode.UpdateStats(dragStart, mousemotion.Position, dragging);
        }
    }


}
