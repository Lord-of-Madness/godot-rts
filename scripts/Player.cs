using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using RtsZápoèák.UI;
using RtsZápoèák.Gameplay;

namespace RtsZápoèák.mainspace
{
    public partial class Player : Node
    {
        public int ID { get; private set; } = 3;
        public uint BitID//WIP THIS can be done like in the editor via the flag export
        {
            get => ((uint)1) << (ID - 1);
        }

        private SortedSet<Unit> selectedUnits;
        private SelectRect selectRectNode;
        private Node2D localLevel;
        private Camera2D camera;
        public int MAX_SELECTED_THINGS = 99999;

        [Export(PropertyHint.Range, "0,20,1,or_greater")]
        public float ScrollSpeed = 5;

        CanvasLayer HUD;
        private ColorRect topbar;
        private ColorRect bottombar;
        private TextureRect unitPortrait;
        private GridContainer unitsSelectedNode;

        private Selectable hoveringOver;

        enum ClickMode
        {
            Move,
            Attack,
            Patrol,
            Defend
        }
        private ClickMode clickMode = ClickMode.Move;
        public override void _Ready()
        {
            camera = GetNode<Camera2D>(nameof(Camera2D));
            HUD = camera.GetNode<CanvasLayer>(nameof(HUD));
            selectRectNode = GetNode<SelectRect>("SelectRect");
            localLevel = GetParent<Node2D>();
            selectedUnits = new();
            topbar = HUD.GetNode<ColorRect>("TopBar");
            bottombar = HUD.GetNode<ColorRect>("BottomBar");
            unitPortrait = bottombar.GetNode<TextureRect>("UnitPortrait");

            camera.VisibilityLayer = BitID;//I am not sure this is working proper
            unitsSelectedNode = bottombar.GetNode<GridContainer>("UnitsSelected");
            for (int i = 0; i < unitsSelectedNode.Columns; i++)
            {
                unitsSelectedNode.AddChild(new TextureRect()
                {
                    ExpandMode = TextureRect.ExpandModeEnum.FitHeight,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                    SizeFlagsVertical = Control.SizeFlags.ExpandFill
                });
            }
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            var mouseposition = GetViewport().GetMousePosition();
            var screenSize = GetViewport().GetVisibleRect().Size;
            //GD.Print(screenSize);
            //GD.Print(mouseposition);

            if (mouseposition.X == screenSize.X - 1)
            {
                camera.Translate(Vector2.Right * ScrollSpeed);
            }
            else if (mouseposition.X == 0)
            {
                camera.Translate(Vector2.Left * ScrollSpeed);
            }
            if (mouseposition.Y == screenSize.Y - 1)
            {
                camera.Translate(Vector2.Down * ScrollSpeed);
            }
            else if (mouseposition.Y == 0)
            {
                camera.Translate(Vector2.Up * ScrollSpeed);
            }

        }
        public void TogglePause(bool toggleOn)
        {
            Input.MouseMode = toggleOn ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Confined;
            localLevel.GetTree().Paused = toggleOn;
        }

        public override void _UnhandledInput(InputEvent @event)
        {

            base._UnhandledInput(@event);
            if (@event is InputEventMouseButton mousebutton)
                if (mousebutton.ButtonIndex == MouseButton.Left)
                {
                    SelectUnits(mousebutton);
                }
                else if (mousebutton.ButtonIndex == MouseButton.Right && mousebutton.Pressed)
                {
                    //Here should be a check whether mousebutton.Position is a location or a hostile entity.
                    switch (clickMode)
                    {
                        case ClickMode.Move:
                            foreach (Unit unit in selectedUnits)
                            {
                                unit.MoveTo(mousebutton.Position);
                            }
                            break;
                        case ClickMode.Attack:
                            foreach (Unit unit in selectedUnits)
                            {
                                unit.AttackCommand(mousebutton.Position);
                            }
                            break;
                    }
                }
            if (selectRectNode.dragging && @event is InputEventMouseMotion mousemotion)
            {
                selectRectNode.UpdateStats(mousemotion.Position);
            }
        }
        public void SelectUnits(InputEventMouseButton mousebutton)
        {
            if (mousebutton.Pressed)//Just pressed
            {
                selectRectNode.dragging = true;
                selectRectNode.start = mousebutton.Position;
                if (selectedUnits.Count > 0 && !Input.IsKeyPressed(Key.Shift))//The shift key is kinda hardcoded perhaps change that later
                {
                    foreach (object shape in selectedUnits)
                    {
                        if (shape is Unit unit)
                        {
                            unit.Deselect();

                        }
                    }
                    selectedUnits = new();
                    foreach (TextureRect child in unitsSelectedNode.GetChildren())
                        child.Texture = null;//TODO: probably make it that there are the texture spots already premade and we just add textures to them. I don't like this construction and desctruction of nodes
                    unitPortrait.Texture = null;
                }
            }
            else if (selectRectNode.dragging)//Just released (The mouseevent triggers only when pressing/releasing this just ensures it was dragging before)
            {
                selectRectNode.dragging = false;
                var dragEnd = mousebutton.Position;
                selectRectNode.UpdateStats(dragEnd);
                PhysicsShapeQueryParameters2D query = new()
                {
                    Shape = new RectangleShape2D() { Size = selectRectNode.Size.Abs() },
                    Transform = new Transform2D(0, (dragEnd + selectRectNode.start) / 2)
                };


                foreach (Dictionary shape in localLevel.GetWorld2D().DirectSpaceState.IntersectShape(query, MAX_SELECTED_THINGS))
                {
                    if (((GodotObject)shape["collider"]) is Unit unit)
                    {
                        if (selectedUnits.Add(unit))//isn't selected again (shift select) 
                        {
                            unit.Select();//The unit shouldn't care about being selected. This should be handled differently(visibility to the camera I pressume)
                        }
                    }

                }
                if(selectedUnits.Count == 0)
                {
                    //TODO select other things than units -> buildings?
                }
                var suEnum = selectedUnits.GetEnumerator();
                for (int i = 0; i < Math.Min(unitsSelectedNode.Columns, selectedUnits.Count); i++)
                {
                    //this feels like the best way to do it while keeping the sortedSet. (Eventually could change it to SortedList I guess and just index into it)
                    suEnum.MoveNext();
                    ((TextureRect)unitsSelectedNode.GetChild(i)).Texture = suEnum.Current.GetNode<Sprite2D>("UnitPortrait").Texture;
                }
                if (selectedUnits.Count > 0)
                    unitPortrait.Texture = selectedUnits.Min.GetNode<Sprite2D>("UnitPortrait").Texture;//No need to have UnitPortrait as part of the unit itself. Can be external resource. THough maybe its safer?
            }
        }

    }
}