using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using RTS.UI;
using RTS.mainspace;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading.Tasks;

namespace RTS.Gameplay
{
    //[StructLayout(LayoutKind.Explicit)]//Tried making it into C++esque  Variant-like object. Probably silly and completely unecessary. And it didn't even work(even making it a struct broke something somewhere)
    public class Target
    {
        public enum Type
        {
            Location,
            Selectable
        }
        public Target(){}
        public Target(Target other)
        {
            type = other.type;
            selectable = other.selectable;
            location = other.location;
        }
        //[FieldOffset(0)]
        public Type type;
        //[FieldOffset(sizeof(Type))]
        public Selectable selectable;
        //[FieldOffset(sizeof(Type))]
        public Vector2 location;
        public Vector2 Position { get
            {
                if (type == Type.Location) return location;
                else return selectable.Position;
            } }
    }
    public partial class Player : Node
    {
        public int ID { get; private set; } = 3;
        public uint BitID//WIP THIS can be done like in the editor via the flag export
        {
            get => ((uint)1) << (ID - 1);
        }
        public string name = "Player";

        private SortedSet<Unit> selectedUnits;
        private SelectRect selectRectNode;
        private Node2D localLevel;
        private Camera2D camera;
        public int MAX_SELECTED_THINGS = 99999;

        [Export(PropertyHint.Range, "0,20,1,or_greater")]
        public float ScrollSpeed = 5;

        CanvasLayer HUD;
        private ColorRect TopBar;
        private ColorRect BottomBar;
        private TextureRect UnitPortrait;
        private GridContainer UnitsSelected;

        private Target hoveringOver;

        public void JustHovered(Selectable target)
        {
            hoveringOver.type = Target.Type.Selectable;
            hoveringOver.selectable = target;
            ((Unit)target).Graphics.Hover();
        }
        public void DeHovered(Selectable target)
        {
            hoveringOver.type = Target.Type.Location;
            hoveringOver.selectable = null;
            ((Unit)target).Graphics.DeHover();
        }

        public enum ClickMode
        {
            Move,
            Attack,
            Patrol,
            Defend
        }
        private ClickMode clickMode = ClickMode.Move;
        public override void _Ready()
        {
            hoveringOver = new();
            camera = GetNode<Camera2D>(nameof(Camera2D));
            HUD = camera.GetNode<CanvasLayer>(nameof(HUD));
            selectRectNode = HUD.GetNode<SelectRect>(nameof(SelectRect));
            localLevel = GetParent<Node2D>();
            selectedUnits = new();
            TopBar = HUD.GetNode<ColorRect>(nameof(TopBar));
            BottomBar = HUD.GetNode<ColorRect>(nameof(BottomBar));
            UnitPortrait = BottomBar.GetNode<TextureRect>(nameof(UnitPortrait));

            camera.VisibilityLayer = BitID;//I am not sure this is working proper
            UnitsSelected = BottomBar.GetNode<GridContainer>(nameof(UnitsSelected));
            for (int i = 0; i < UnitsSelected.Columns; i++)
            {
                UnitsSelected.AddChild(new TextureRect()
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

            if (mouseposition.X >= screenSize.X - 1)
            {
                camera.Translate(Vector2.Right * ScrollSpeed);
                camera.Position = new Vector2(Math.Min(camera.Position.X, camera.LimitRight - camera.GetViewportRect().Size.X), camera.Position.Y);

                //This is so stupid. The events that I am getting are coming based off cameraNODE coordinates.
                //These coordinates are being translated by pushing mouse to the sides of the screen.
                //However the nodes actual coordinates aren't confined to the Limits of the camera. And not even to the top left corner of the viewport.
                //so I am, after every translation, moving it back into the Topleft corner of the viewport within limits.
                //For some reason GloabalPosition of anything is just Position and this is the only way of getting them real numbers.
                //I mean there has to be a better way I just didn't find it

            }
            else if (mouseposition.X <= 0)
            {
                camera.Translate(Vector2.Left * ScrollSpeed);
                camera.Position = new Vector2(Math.Max(camera.Position.X, camera.LimitLeft), camera.Position.Y);
            }
            if (mouseposition.Y >= screenSize.Y - 1)
            {
                camera.Translate(Vector2.Down * ScrollSpeed);
                camera.Position = new Vector2(camera.Position.X, Math.Min(camera.Position.Y, camera.LimitBottom - camera.GetViewportRect().Size.Y));
            }
            else if (mouseposition.Y <= 0)
            {
                camera.Translate(Vector2.Up * ScrollSpeed);
                camera.Position = new Vector2(camera.Position.X, Math.Max(camera.Position.Y, camera.LimitTop));
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

                    if (hoveringOver.type == Target.Type.Location)
                    {
                        //hoveringOver.location = mousebutton.GlobalPosition+camera.Position;
                        hoveringOver.location = GetViewport().GetMousePosition() + camera.Position;
                    }
                    else if(hoveringOver.type == Target.Type.Selectable && hoveringOver.selectable.team.IsHostile())
                    {
                        clickMode=ClickMode.Attack;
                    }
                    /*Parallel.ForEach(selectedUnits, unit => {
                        if (!mousebutton.ShiftPressed) { unit.CleanCommandQueue(); }
                        unit.Command(clickMode, hoveringOver);
                    });*/
                    //Throws odd errors
                    foreach (Unit unit in selectedUnits)//Can be paralelised
                    {
                        if (!mousebutton.ShiftPressed) { unit.CleanCommandQueue(); }
                        unit.Command(clickMode, new Target(hoveringOver));
                    }
                    if (!mousebutton.ShiftPressed) clickMode = ClickMode.Move;


                }
            if (@event is InputEventKey key)
            {
                if (key.Keycode == Key.A)
                {
                    clickMode = ClickMode.Attack;
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
                    foreach (TextureRect child in UnitsSelected.GetChildren().Cast<TextureRect>())
                        child.Texture = null;//TODO: probably make it that there are the texture spots already premade and we just add textures to them. I don't like this construction and desctruction of nodes
                    UnitPortrait.Texture = null;
                }
            }
            else if (selectRectNode.dragging)//Just released (The mouseevent triggers only when pressing/releasing this just ensures it was dragging before)
            {
                selectRectNode.dragging = false;
                var dragEnd = mousebutton.Position;
                selectRectNode.UpdateStats(dragEnd);
                PhysicsShapeQueryParameters2D query = new()
                {
                    Shape = new RectangleShape2D() { Size = selectRectNode.Size },
                    Transform = new Transform2D(0, camera.GlobalPosition + (dragEnd + selectRectNode.start) / 2)
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
                if (selectedUnits.Count == 0)
                {
                    //TODO select other things than units -> buildings?
                }
                var suEnum = selectedUnits.GetEnumerator();
                for (int i = 0; i < Math.Min(UnitsSelected.Columns, selectedUnits.Count); i++)
                {
                    //this feels like the best way to do it while keeping the sortedSet. (Eventually could change it to SortedList I guess and just index into it)
                    suEnum.MoveNext();
                    ((TextureRect)UnitsSelected.GetChild(i)).Texture = suEnum.Current.GetNode<Sprite2D>("UnitPortrait").Texture;
                }
                if (selectedUnits.Count > 0)
                    UnitPortrait.Texture = selectedUnits.Min.GetNode<Sprite2D>("UnitPortrait").Texture;//No need to have UnitPortrait as part of the unit itself. Can be external resource. THough maybe its safer?
            }
        }

    }
}