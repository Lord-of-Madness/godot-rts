using Godot;
using System;
using RTS.Gameplay;
using RTS.mainspace;

namespace RTS.Graphics
{
    public partial class SelectableGraphics : Node2D
    {
        public Line2D PathLine;
        protected Node2D SelectionVisual;
        protected Color selectionSelfModulateColor;
        protected bool selected;

        protected AnimationPlayer anim;
        public static readonly float SQRT2 = (float)Math.Sqrt(2);
        protected Selectable parent;
        protected const string DEATH = "Death";
        protected bool IsDead { get => anim.CurrentAnimation == DEATH; }
        public Direction Direction { get; protected set; } = Direction.Forward;
        public override void _Ready()
        {
            SelectionVisual = GetNode<Node2D>("Selected");
            selectionSelfModulateColor = SelectionVisual.SelfModulate;
            anim = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
            PathLine = GetNode<Line2D>(nameof(PathLine));
            PathLine.DefaultColor = new Color(0, 1, 0, 0.3f);
            parent = GetParent<Selectable>();
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
        public void DeathAnim()
        {
            anim.Play(DEATH);
            anim.AnimationFinished += (_) => parent.QueueFree();
        }
    }
}
