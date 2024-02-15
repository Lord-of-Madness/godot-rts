using Godot;
using System;
using RTS.mainspace;
using System.Threading.Tasks;

namespace RTS.Graphics
{
    /// <summary>
    /// This should be the unique graphical interface of a Unit and should be unique to each player/viewport
    /// </summary>
    public partial class UnitGraphics : SelectableGraphics
    {
        public NavigationAgent2D NavAgent;
        
        public Direction Direction { get; private set; }



        public override void _Ready()
        {
            base._Ready();
            NavAgent = parent.GetNode<NavigationAgent2D>(nameof(NavAgent));//this has to be this way cause this gets instantiated earlier than parents navagent (making it parent.navagent null)
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
            PathLine.GlobalPosition = Vector2.Zero;//So path doesn't move with the unit
            if (points.Length != 0) pts[0] = parent.GlobalPosition;//We don't want to keep drawing the location behind us
            PathLine.Points = pts;

        }
        public void NavigationFinished()
        {
            if(!IsDead) anim.Play($"Idle{Direction}");
            PathLine.Points = Array.Empty<Vector2>();
            //Sometimes navigation is finished even when not all points have been passed through
        }



    }
}
