using Godot;
using Godot.Collections;
using RTS.Gameplay;
using RTS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RTS.Gameplay.Player;

namespace RTS.scripts.Gameplay
{

    public partial class HumanPlayer : Player
    {
        public static class Cursors
        {
            private static Resource target;
            private static Resource hand;

            public static Resource Target { get => target; set => target = value; }
            public static Resource Hand { get => hand; set => hand = value; }

            static Cursors()
            {
                Target = ResourceLoader.Load("res://assets/MouseIcons/target.png");
                Hand = ResourceLoader.Load("res://assets/MouseIcons/hand.png");
            }

        }

        private SelectRect selectRectNode;
        private Camera2D camera;
        public int MAX_SELECTED_THINGS = 99999;
        private CanvasLayer HUD;
        private ColorRect TopBar;
        private ColorRect BottomBar;
        private TextureRect UnitPortrait;
        private GridContainer UnitsSelected;
        private UnitActions UnitActions;
        [Export(PropertyHint.Range, "0,20,1,or_greater")]
        public float ScrollSpeed = 5;
        private Target hoveringOver;
        private HBoxContainer ResourceTab;

        private ClickMode cm;
        public ClickMode Clickmode
        {
            get { return cm; }
            set
            {
                cm = value;
                switch (value)
                {
                    case ClickMode.Move:
                        Input.SetCustomMouseCursor(Cursors.Hand, hotspot: new Vector2(16, 0));
                        break;
                    case ClickMode.Attack:
                    case ClickMode.UseAbility:
                        Input.SetCustomMouseCursor(Cursors.Target, hotspot: new Vector2(16, 16));
                        break;
                    default:
                        GD.PrintErr("Not implemented cursor thingie - using default");
                        Input.SetCustomMouseCursor(Cursors.Hand, hotspot: new Vector2(0, 16));
                        break;
                }
            }
        }

        public void JustHovered(Selectable target)
        {
            hoveringOver.type = Target.Type.Selectable;
            hoveringOver.selectable = target;
            target.Graphics.Hover();
        }
        public void DeHovered(Selectable target)
        {
            hoveringOver.type = Target.Type.Location;
            hoveringOver.selectable = null;
            target.Graphics.DeHover();
        }
        public override void _Ready()
        {
            base._Ready();
            Clickmode = ClickMode.Move;
            hoveringOver = new();
            camera = GetNode<Camera2D>(nameof(Camera2D));
            HUD = camera.GetNode<CanvasLayer>(nameof(HUD));
            selectRectNode = HUD.GetNode<SelectRect>(nameof(SelectRect));
            TopBar = HUD.GetNode<ColorRect>(nameof(TopBar));
            ResourceTab = TopBar.GetNode<HBoxContainer>(nameof(ResourceTab));
            /*foreach (var res in Game_Resources)
            {
                ResourceTab.AddChild(res);
            }*/
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
            UnitActions = BottomBar.GetNode<UnitActions>(nameof(UnitActions));
        }
        private void CameraMovement()
        {
            var mouseposition = GetViewport().GetMousePosition();
            var screenSize = GetViewport().GetVisibleRect().Size;
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


                //Test if if we add a rectangle as a view screen in we can nudge it around.

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
        public override void _Process(double delta)
        {
            base._Process(delta);
            /*if (Engine.IsEditorHint())
            {
                if (!EditorPause)
                {
                    GD.Print(GetNode<CanvasLayer>(nameof(Camera2D) + "/" + nameof(HUD)).GetNode<ColorRect>(nameof(TopBar)).GetNode<HBoxContainer>(nameof(ResourceTab)));
                }
            }
            else
            {*/
            CameraMovement();
            //}

        }
        public override void _UnhandledInput(InputEvent @event)
        {

            base._UnhandledInput(@event);
            if (@event is InputEventMouseButton mousebutton)
                if (mousebutton.ButtonIndex == MouseButton.Left)
                {
                    Clickmode = ClickMode.Move;
                    SelectObjects(mousebutton);
                }
                else if (mousebutton.ButtonIndex == MouseButton.Right && mousebutton.Pressed)
                {

                    if (hoveringOver.type == Target.Type.Location)
                    {
                        //hoveringOver.location = mousebutton.GlobalPosition+camera.Position;
                        hoveringOver.location = GetViewport().GetMousePosition() + camera.Position;
                    }
                    else if (hoveringOver.type == Target.Type.Selectable && hoveringOver.selectable.team.IsHostile())
                    {
                        Clickmode = ClickMode.Attack;
                    }
                    /*Parallel.ForEach(selectedUnits, unit => {
                        if (!mousebutton.ShiftPressed) { unit.CleanCommandQueue(); }
                        unit.Command(clickMode, hoveringOver);
                    });*/
                    //Throws odd errors
                    foreach (Selectable selectable in Selection)//Can be paralelised
                    {
                        if (!mousebutton.ShiftPressed) { selectable.CleanCommandQueue(); }
                        selectable.Command(Clickmode, new Target(hoveringOver));
                    }
                    if (!mousebutton.ShiftPressed) Clickmode = ClickMode.Move;


                }
            if (@event is InputEventKey key)
            {
                if (key.Keycode == Key.A)
                {
                    Clickmode = ClickMode.Attack;
                }
            }
            if (selectRectNode.dragging && @event is InputEventMouseMotion mousemotion)
            {
                selectRectNode.UpdateStats(mousemotion.Position);
            }
        }
        public void SelectObjects(InputEventMouseButton mousebutton)
        {
            if (mousebutton.Pressed)//Just pressed
            {
                selectRectNode.dragging = true;
                selectRectNode.start = mousebutton.Position;
                if (Selection.Count > 0 && !Input.IsKeyPressed(Key.Shift))//The shift key is kinda hardcoded perhaps change that later
                {
                    foreach (Selectable selectable in Selection)
                    {
                        selectable.Deselect();
                        selectable.SignalDisablingSelection -= DeselectObject;
                        //specificaly not using DeselectUnit(unit) cause I want to reset tbe selected units and not remove them one after another
                    }
                    Selection = new();

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


                var prefiltered = (from s in localLevel.GetWorld2D().DirectSpaceState.IntersectShape(query, MAX_SELECTED_THINGS)
                                   where ((GodotObject)s["collider"]) is Selectable
                                   select (Selectable)s["collider"]).ToArray();
                if (prefiltered.Length == 1)
                {

                    if (Selection.Add(prefiltered.First())) SelectObject(prefiltered.First());
                }
                else
                {
                    foreach (Selectable selectable in prefiltered)
                    {
                        if (
                            selectable is Unit unit
                            &&
                            (unit.CurrentAction != Unit.UnitAction.Dying)
                            &&
                            Selection.Add(unit)//isn't selected again (shift select) 
                            &&
                            unit.team == team
                            )
                        {
                            SelectObject(unit);
                        }

                    }
                }
                UpdateUnitGridAndPortrait();


            }
        }
        protected void SelectObject(Selectable selectable)
        {
            selectable.Select();//The unit shouldn't care about being selected. This should be handled differently(visibility to the camera I pressume)
            selectable.SignalDisablingSelection += DeselectObject;
        }
        private void DeselectObject(Selectable selectable)
        {
            selectable.SignalDisablingSelection -= DeselectObject;
            Selection.Remove(selectable);
            selectable.Deselect();
            UpdateUnitGridAndPortrait();
        }
        private void UpdateUnitGridAndPortrait()
        {
            //TODO: IF only one selected then UNITCARD
            foreach (TextureRect child in UnitsSelected.GetChildren().Cast<TextureRect>())
                child.Texture = null;//TODO: probably make it that there are the texture spots already premade and we just add textures to them. I don't like this construction and desctruction of nodes
            UnitPortrait.Texture = null;
            var suEnum = Selection.GetEnumerator();
            for (int i = 0; i < Math.Min(UnitsSelected.Columns, Selection.Count); i++)
            {
                //this feels like the best way to do it while keeping the sortedSet. (Eventually could change it to SortedList I guess and just index into it)
                suEnum.MoveNext();
                ((TextureRect)UnitsSelected.GetChild(i)).Texture = suEnum.Current.GetNode<Sprite2D>(nameof(UnitPortrait)).Texture;
            }


            if (Selection.Count > 0)
            {
                Selectable HighlightedUnit = Selection.Min;//TODO: It is the smallest for now but that is only default it should be Tabed through
                UnitPortrait.Texture = HighlightedUnit.GetNode<Sprite2D>(nameof(UnitPortrait)).Texture;//No need to have UnitPortrait as part of the unit itself. Can be external resource. THough maybe its safer?
                UnitActions.FillGridButtons(HighlightedUnit.Abilities,this);
}
            else
            {
                UnitActions.DestroyChildren();
            }



            
        }
        
    }
}
