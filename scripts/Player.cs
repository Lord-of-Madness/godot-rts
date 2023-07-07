using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using RTSUI;
using RTSGameplay;
using GDArray = Godot.Collections.Array;

namespace RTSmainspace
{
    public partial class Player : Node
    {
        private List<Unit> selectedUnits;
        private SelectRect selectRectNode;
        private Node2D localLevel;
        public int MAX_SELECTED_THINGS = 99999;
        public override void _Ready()
        {
            selectRectNode = GetNode<SelectRect>("SelectRect");
            localLevel = GetParent<Node2D>();
            selectedUnits = new();

        }
        public void TogglePause(bool toggleOn)
        {
            localLevel.GetTree().Paused = toggleOn;
        }

        public override void _UnhandledInput(InputEvent @event)
        {

            base._UnhandledInput(@event);
            if (@event is InputEventMouseButton mousebutton)
                if (mousebutton.ButtonIndex == MouseButton.Left)
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
                        selectedUnits = new();
                        selectRectNode.dragging = true;
                        selectRectNode.start = mousebutton.Position;
                    }
                    else if (selectRectNode.dragging)
                    {
                        selectRectNode.dragging = false;
                        var dragEnd = mousebutton.Position;
                        selectRectNode.UpdateStats(dragEnd);
                        PhysicsShapeQueryParameters2D query = new()
                        {
                            Shape = new RectangleShape2D() { Size = selectRectNode.Size.Abs() },
                            Transform = new Transform2D(0, (dragEnd + selectRectNode.start) / 2)
                        };
                        foreach (Dictionary shape in localLevel.GetWorld2D().DirectSpaceState.IntersectShape(query,MAX_SELECTED_THINGS))
                        {
                            if (((GodotObject)shape["collider"]) is Unit unit)
                            {
                                unit.Select();
                                selectedUnits.Add(unit);
                            }

                        }
                    }
                }
                else if (mousebutton.ButtonIndex == MouseButton.Right && mousebutton.Pressed)
                {
                    foreach (Unit unit in selectedUnits)
                    {
                        unit.MoveTo(mousebutton.Position);
                    }
                }
            if (selectRectNode.dragging && @event is InputEventMouseMotion mousemotion)
            {
                selectRectNode.UpdateStats(mousemotion.Position);
            }
        }


    }
}