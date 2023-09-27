using Godot;
using System;
using RTS.Gameplay;
using RTS.mainspace;
using System.Threading.Tasks;

namespace RTS.Graphics
{
    /// <summary>
    /// This should be the unique graphical interface of a Unit and should be unique to each player/viewport
    /// </summary>
    public partial class UnitGraphics : Node
    {
        private Node2D SelectionVisual;
        private Color selectionSelfModulateColor;
        public bool selected;

        private AnimationPlayer anim;
        public static readonly float SQRT2 = (float)Math.Sqrt(2);
        public NavigationAgent2D navAgent;
        private Line2D path;
        private Selectable parent;
        public Direction Direction { get; private set; }



        public override void _Ready()
        {
            SelectionVisual = GetNode<Node2D>("Selected");
            selectionSelfModulateColor = SelectionVisual.SelfModulate;
            anim = GetNode<AnimationPlayer>("AnimationPlayer");
            path = GetNode<Line2D>("PathLine");
            path.DefaultColor = new Color(0, 1, 0, 0.3f);
            //GetNode<Sprite2D>("Sprite2D").Frame = 1;//This might be removed later what is this anyway?
            parent = GetParent<Selectable>();
            navAgent = parent.GetNode<NavigationAgent2D>("NavAgent");//this has to be this way cause this gets instantiated earlier than parents navagent (making it parent.navagent null)
            selected = false;
        }

        public void Hover()
        {
            if (!selected)
            {
                SelectionVisual.Visible = true;
                Color c = selectionSelfModulateColor;
                c.A /= 2;
                SelectionVisual.SelfModulate = c;
            }
        }
        public void DeHover()
        {
            if (!selected)
            {
                SelectionVisual.SelfModulate = selectionSelfModulateColor;
                SelectionVisual.Visible = false;
            }

        }
        public void Select()
        {
            DeHover();
            selected = true;
            SelectionVisual.Visible = true;
            path.Visible = true;

        }
        public void Deselect()
        {
            selected = false;
            SelectionVisual.Visible = false;
            path.Visible = false;

        }

        internal void MovingTo(Vector2 direction)
        {
            if (anim.CurrentAnimation != "Death") return;
            //Sprite:
            if (direction.X > 0.5)
            {
                anim.Play(Direction.Right.ToString());
                Direction = Direction.Right;
            }
            else if (direction.X < -0.5)
            {
                anim.Play(Direction.Left.ToString());
                Direction = Direction.Left;
            }
            else if (direction.Y < -0.5)
            {
                anim.Play(Direction.Back.ToString());
                Direction = Direction.Back;
            }
            else
            {
                anim.Play(Direction.Forward.ToString());
                Direction = Direction.Forward;
            }

            //Path:
            Vector2[] points = navAgent.GetCurrentNavigationPath();

            if (points.Length == 0) { path.Points = Array.Empty<Vector2>(); return; }

            int from = navAgent.GetCurrentNavigationPathIndex();
            Vector2[] pts = new Vector2[points.Length - from];
            for (int i = 0; i < pts.Length; i++) pts[i] = points[i + from];
            path.GlobalPosition = Vector2.Zero;//So path doesn't move with player
            if (points.Length != 0) pts[0] = parent.GlobalPosition;//We don't want to keep drawing the location behind us
            path.Points = pts;

        }
        public void NavigationFinished()
        {
            if(anim.CurrentAnimation!= "Death") anim.Play($"Idle{Direction}");
            path.Points = Array.Empty<Vector2>();
            //Sometimes navigation is finished even when not all points have been passed through
        }

        public void DeathAnim()
        {
            anim.Play("Death");
            anim.AnimationFinished += (_)=>parent.QueueFree();

        }

    }
}
