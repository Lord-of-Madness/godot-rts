using Godot;
using RTS.UI;
using System;
using System.Linq;
using RTS.mainspace;
using System.Collections.Generic;
using static RTS.Gameplay.Player;
using System.Collections;

namespace RTS.Gameplay
{
    public class Selection : IEnumerable<Selectable>
    {
        SortedSet<Selectable> selection;
        public SortedSet<Selectable> highlightedGroup;
        public Selectable highlightedSelectable;
        public int Count { get => selection.Count; }
        public Selection()
        {
            selection = new SortedSet<Selectable>();
            highlightedGroup = new SortedSet<Selectable>();
        }
        public bool Add(Selectable selectable)
        {
            if (!selection.Add(selectable)) return false;
            if (highlightedGroup.Count == 0 || highlightedGroup.First().SName == selectable.SName)
            //We add to the group if it was empty or if the added element has the same type - SName
            {
                highlightedGroup.Add(selectable);//If we chose to bother the highlighted group is always next to each other in selection so it could be just indexes
            }
            highlightedSelectable ??= selectable;
            return true;
        }
        public void Add(IEnumerable<Selectable> selectables)//Here we might be able to optimize if we wanted to.
        {
            foreach (Selectable selectable in selectables)
            {
                Add(selectable);
            }
        }
        public bool Remove(Selectable selectable)
        {
            if (!selection.Remove(selectable)) return false;
            if (highlightedGroup.Remove(selectable))
            {

                if (highlightedGroup.Count == 0)
                {
                    SetUpNewGroup(selection.First().SName);
                }



                if (highlightedSelectable == selectable)//makes sense to undo the one selectable if it was in the group before
                {
                    highlightedSelectable = highlightedGroup.First();//pick a new selectable from the newly redone highlighted group
                }
            }
            return true;//doesn't matter what highlighted group does if it worked in selection we are fine
        }
        void SetUpNewGroup(string sname, bool resetGroup = false)
        {
            if (resetGroup) highlightedGroup.Clear();
            var en = selection.GetEnumerator();
            while (en.MoveNext() && en.Current.SName == sname)
            {
                highlightedGroup.Add(en.Current);
            }
        }

        public IEnumerator<Selectable> GetEnumerator()
        {
            return selection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void Clear()
        {
            selection.Clear();
            highlightedGroup.Clear();
            highlightedSelectable = null;
        }
    }
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
        public Camera2D camera;
        /// <summary>
        /// Specifies how many objects will the SelecRect return. Technicaly We want infinty so we can grab all and any unit but... 
        /// </summary>
        public int MAX_SELECTED_THINGS = 99999;
        private CanvasLayer HUD;
        private ColorRect TopBar;
        private ColorRect BottomBar;
        private TextureRect UnitPortrait;
        private InfoContainer InfoContainer;
        private UnitActions UnitActions;
        private Vector2 MinZoom;
        private Vector2 MaxZoom = new(3f, 3f);
        private Vector2 Zoom
        {
            get => camera.Zoom;
            set
            {
                value = value.Clamp(MinZoom, MaxZoom);
                camera.Zoom = value;
            }
        }
        [ExportGroup("CameraProperties")]
        [Export(PropertyHint.Range, "0,20,1,or_greater")]
        public float ScrollSpeed = 5;
        [ExportSubgroup("Zoom")]
        [Export(PropertyHint.Range, "1.01,5,0.1,or_greater")]
        public float ZoomSnapAmmount = 1.1f;
        [Export(PropertyHint.Range, "1,20,1,or_greater")]
        float ZoomSpeed = 10;
        [Export]
        bool ZoomSnap = false;
        private Vector2 TargetZoom;
        private ITargetable hoveringOver;
        private HBoxContainer ResourceTab;

        public TargetedAbility HangingAbility = null;
        private ClickMode cm;
        /// <summary>
        /// Specifies what action are we currently giving the Selected Selectables.
        /// </summary>
        public ClickMode Clickmode
        {
            get { return cm; }
            set
            {
                cm = value;
                switch (value)
                {
                    case ClickMode.Move:
                        Input.SetCustomMouseCursor(Cursors.Hand, hotspot: new(16, 0));
                        break;
                    case ClickMode.Attack:
                    case ClickMode.UseAbility:
                        Input.SetCustomMouseCursor(Cursors.Target, hotspot: new(16, 16));
                        break;
                    default:
                        GD.PrintErr("Not implemented cursor thingie - using default");
                        Input.SetCustomMouseCursor(Cursors.Hand, hotspot: new(0, 16));
                        break;
                }
            }
        }
        /// <summary>
        /// Currently selected <c>Selectable</c>s
        /// </summary>
        protected Selection Selection = new();

        public void JustHovered(Selectable target)
        {
            hoveringOver = target;
            target.Graphics.Hover();
        }
        public void DeHovered(Selectable target)
        {
            hoveringOver = new Location(Vector2.Zero);
            target.Graphics.DeHover();
        }
        public override void _Ready()
        {
            base._Ready();
            Clickmode = ClickMode.Move;
            hoveringOver = new Location(Vector2.Zero);
            camera = GetNode<Camera2D>(nameof(Camera2D));
            HUD = camera.GetNode<CanvasLayer>(nameof(HUD));
            selectRectNode = HUD.GetNode<SelectRect>(nameof(SelectRect));
            TopBar = HUD.GetNode<ColorRect>(nameof(TopBar));
            ResourceTab = TopBar.GetNode<HBoxContainer>(nameof(ResourceTab));
            BottomBar = HUD.GetNode<ColorRect>(nameof(BottomBar));
            UnitPortrait = BottomBar.GetNode<TextureRect>(nameof(UnitPortrait));

            var minDimension = Mathf.Max(camera.GetViewportRect().Size.Y/ camera.LimitHeight(), camera.GetViewportRect().Size.X/ camera.LimitWidth());
            MinZoom = new(minDimension, minDimension);//NOTE: this means it will always keep the same dimensions based on zoom. Might wanna change it someday
            TargetZoom = Zoom;

            //camera.VisibilityLayer = BitID;//I am not sure this is working proper
            InfoContainer = BottomBar.GetNode<InfoContainer>(nameof(InfoContainer));
            UnitActions = BottomBar.GetNode<UnitActions>(nameof(UnitActions));
        }
        /// <summary>
        /// Handles the movements of camera when mouse approaches the sides of the screen
        /// </summary>
        private void CameraMovement()
        {
            var mouseposition = GetViewport().GetMousePosition();
            var screenSize = GetViewport().GetVisibleRect().Size;
            if (mouseposition.X >= screenSize.X - 1)
            {
                camera.Translate(Vector2.Right * ScrollSpeed /camera.Zoom);
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
                camera.Translate(Vector2.Left * ScrollSpeed / camera.Zoom);
                camera.Position = new Vector2(Math.Max(camera.Position.X, camera.LimitLeft), camera.Position.Y);
            }
            if (mouseposition.Y >= screenSize.Y - 1)
            {
                camera.Translate(Vector2.Down * ScrollSpeed / camera.Zoom);
                camera.Position = new Vector2(camera.Position.X, Math.Min(camera.Position.Y, camera.LimitBottom - camera.GetViewportRect().Size.Y));
            }
            else if (mouseposition.Y <= 0)
            {
                camera.Translate(Vector2.Up * ScrollSpeed / camera.Zoom);
                camera.Position = new Vector2(camera.Position.X, Math.Max(camera.Position.Y, camera.LimitTop));
            }
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            //camera.Zoom.Lerp() //TODO: https://www.youtube.com/watch?v=lS_k94RyKd4&ab_channel=DevWorm
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
            if(!ZoomSnap)
                Zoom = Zoom.Lerp(TargetZoom, (float)delta*10);
            InfoContainer.Update(Selection);
            //}

        }
        public override void _UnhandledInput(InputEvent @event)
        {

            base._UnhandledInput(@event);
            if (@event is InputEventMouseButton mousebutton)
            {
                if (mousebutton.ButtonIndex == MouseButton.WheelUp)
                {
                    if (ZoomSnap) Zoom *= ZoomSnapAmmount;
                    else TargetZoom *= ZoomSnapAmmount;
                }
                else if (mousebutton.ButtonIndex == MouseButton.WheelDown)
                {
                    if (ZoomSnap) Zoom /= ZoomSnapAmmount;
                    else TargetZoom /= ZoomSnapAmmount;
                }
                else if (mousebutton.ButtonIndex == MouseButton.Left)
                {
                    if (HangingAbility is not null)
                    {
                        HangingAbility.OnTargetingCanceled();
                        HangingAbility = null;
                    }
                    Clickmode = ClickMode.Move;
                    SelectObjects(mousebutton);
                }
                else if (mousebutton.ButtonIndex == MouseButton.Right && mousebutton.Pressed)
                {

                    if (hoveringOver is Location)
                    {
                        //camera.GetGlobalMousePosition()
                        //hoveringOver.location = mousebutton.GlobalPosition+camera.Position;
                        //hoveringOver = new Location(GetViewport().GetMousePosition() + camera.Position); // only good when not zooming
                        //hoveringOver = new Location(DisplayServer.MouseGetPosition());//Weird coordinates
                        hoveringOver = new Location(camera.GetGlobalMousePosition());

                    }
                    else if (hoveringOver is Selectable s && s.team.IsHostile(Team))
                    {
                        Clickmode = ClickMode.Attack;
                    }
                    /*Parallel.ForEach(selectedUnits, unit => {
                        if (!mousebutton.ShiftPressed) { unit.CleanCommandQueue(); }
                        unit.Command(clickMode, hoveringOver);
                    });*/
                    //Throws odd errors
                    if (Clickmode == ClickMode.UseAbility)
                    {
                        Selection.highlightedSelectable.Command(ClickMode.UseAbility, hoveringOver, HangingAbility);
                        Clickmode = ClickMode.Move;
                    }
                    foreach (Selectable selectable in Selection)//Can be paralelised
                    {
                        if (selectable.team != Team) continue;
                        if (!mousebutton.ShiftPressed) { selectable.CleanCommandQueue(); }
                        selectable.Command(Clickmode, hoveringOver);
                    }
                    if (!mousebutton.ShiftPressed) Clickmode = ClickMode.Move;
                }


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
                selectRectNode.UpdateStats(mousemotion.GlobalPosition);
            }
        }
        public void SelectObjects(InputEventMouseButton mousebutton)
        {
            if (mousebutton.Pressed)//Just pressed
            {
                selectRectNode.dragging = true;
                selectRectNode.start = mousebutton.GlobalPosition;// mousebutton.Position;
                if (Selection.Count > 0 && !Input.IsKeyPressed(Key.Shift))//TODO the shift key is kinda hardcoded perhaps change that later
                {
                    foreach (Selectable selectable in Selection)
                    {
                        selectable.Deselect();
                        selectable.SignalDisablingSelection -= DeselectObject;
                        //specificaly not using DeselectObject(selectable) cause I want to reset the selected units and not remove them one after another
                    }
                    Selection.Clear();

                }
            }
            else if (selectRectNode.dragging)//Just released (The mouseevent triggers only when pressing/releasing this just ensures it was dragging before)
            {
                selectRectNode.dragging = false;
                var dragEnd = mousebutton.GlobalPosition;//mousebutton.Position;
                selectRectNode.UpdateStats(dragEnd);
                PhysicsShapeQueryParameters2D query = new()
                {
                    Shape = new RectangleShape2D() { Size = selectRectNode.Size / camera.Zoom },
                    Transform = new Transform2D(0, camera.GlobalPosition + (dragEnd + selectRectNode.start) / 2 / camera.Zoom)
                };

                Selectable[] prefiltered = (from s in localLevel.GetWorld2D().DirectSpaceState.IntersectShape(query, MAX_SELECTED_THINGS)
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
                            (unit.CurrentAction != Selectable.SelectableAction.Dying)
                            &&
                            unit.team == Team
                            &&
                            Selection.Add(unit)//isn't selected again (shift select) 
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
            GD.Print(Selection.Remove(selectable));
            selectable.Deselect();
            UpdateUnitGridAndPortrait();
        }
        private void UpdateUnitGridAndPortrait()
        {
            UnitPortrait.Texture = null;




            if (Selection.Count > 0)//this might be handled by the INFOCONTAINER TOO
            {
                UnitPortrait.Texture = Selection.highlightedSelectable.GetNode<Sprite2D>(nameof(UnitPortrait)).Texture;//No need to have UnitPortrait as part of the unit itself. Can be external resource. THough maybe its safer?
                UnitActions.FillGridButtons(Selection.highlightedSelectable.Abilities);
            }
            else
            {
                UnitActions.DestroyChildren();
            }




        }

    }
}
