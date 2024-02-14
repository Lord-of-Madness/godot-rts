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
        public NavigationAgent2D NavAgent;
        private Line2D PathLine;
        private Selectable parent;
        private const string DEATH = "Death";
        private bool IsDead { get =>anim.CurrentAnimation==DEATH; }
        public Direction Direction { get; private set; }



        public override void _Ready()
        {
            SelectionVisual = GetNode<Node2D>("Selected");
            selectionSelfModulateColor = SelectionVisual.SelfModulate;
            anim = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
            PathLine = GetNode<Line2D>(nameof(PathLine));
            PathLine.DefaultColor = new Color(0, 1, 0, 0.3f);
            parent = GetParent<Selectable>();
            NavAgent = parent.GetNode<NavigationAgent2D>(nameof(NavAgent));//this has to be this way cause this gets instantiated earlier than parents navagent (making it parent.navagent null)
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
            PathLine.Visible = true;

        }
        public void Deselect()
        {
            selected = false;
            SelectionVisual.Visible = false;
            PathLine.Visible = false;

        }

        internal void MovingTo(Vector2 direction)
        {
            if (IsDead) return;
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
            Vector2[] points = NavAgent.GetCurrentNavigationPath();

            if (points.Length == 0) { PathLine.Points = Array.Empty<Vector2>(); return; }

            int from = NavAgent.GetCurrentNavigationPathIndex();
            Vector2[] pts = new Vector2[points.Length - from];
            for (int i = 0; i < pts.Length; i++) pts[i] = points[i + from];
            PathLine.GlobalPosition = Vector2.Zero;//So path doesn't move with player
            if (points.Length != 0) pts[0] = parent.GlobalPosition;//We don't want to keep drawing the location behind us
            PathLine.Points = pts;

        }
        public void NavigationFinished()
        {
            if(!IsDead) anim.Play($"Idle{Direction}");
            PathLine.Points = Array.Empty<Vector2>();
            //Sometimes navigation is finished even when not all points have been passed through
        }

        public void DeathAnim()
        {
            anim.Play(DEATH);
            anim.AnimationFinished += (_)=>parent.QueueFree();
        }

    }
}
