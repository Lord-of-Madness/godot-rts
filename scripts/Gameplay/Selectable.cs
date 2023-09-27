using Godot;
using RTS.Graphics;
using RTS.Physics;
using System;

namespace RTS.Gameplay
{
    public static class TeamExtension
    {
        public static bool IsHostile(this Selectable.Team team)
        {
            return false;
        }
    }
    public abstract partial class Selectable : CharacterBody2D, IComparable<Selectable>
    {
        [Signal] public delegate void SignalDisablingSelectionEventHandler(Unit unit);//when dead, loss of control etc.
        public enum Team
        {

        }

        public UnitGraphics Graphics;
        public Team team;
        [Export]
        public Player owner;
        public NavigationAgent2D NavAgent;

        public Area2D VisionArea;
        [Export(PropertyHint.Range, "0,10,1,or_greater")]
        public float visionRange;//in Tilemeters
        public Tilemeter VisionRange { get => (Tilemeter)visionRange; set { visionRange = (float)value; } }
        /// <summary>
		/// Graphicaly shows the selected status.
		/// </summary>
		public void Select()
        {
            Graphics.Select();
        }
        /// <summary>
		/// Graphicaly deselects.
		/// </summary>
        public void Deselect()
        {
            Graphics.Deselect();
        }
#pragma warning disable IDE1006 // Naming Styles
        public void _on_mouse_entered()
#pragma warning restore IDE1006 // Naming Styles
        {
            owner.JustHovered(this);
        }
#pragma warning disable IDE1006 // Naming Styles
        public void _on_mouse_exited()
#pragma warning restore IDE1006 // Naming Styles
        {
            owner.DeHovered(this);
        }
        public virtual void Command(Player.ClickMode clickMode, Target target)
        {
            //Nothing happens
        }
        public override void _Ready()
        {
            Graphics = GetNode<UnitGraphics>("Graphics");
            NavAgent = GetNode<NavigationAgent2D>(nameof(NavAgent));
            VisionArea = GetNode<Area2D>(nameof(VisionArea));
            ((CircleShape2D)VisionArea.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape).Radius = VisionRange.ToPixels();
        }
        /// <summary>
		/// Tries to compare it if its the same type (<c>Unit</c>/<c>Building</c>) Units are above Buildings. Otherwise sorted by Age
		/// </summary>
        public int CompareTo(Selectable other)
        {
            if (other is Unit otherunit)
            {
                if (this is Building) return -1;
                else if (this is Unit unit) return unit.CompareTo(otherunit);
            }
            else if (other is Building otherbuilding)
            {
                if (this is Unit) return 1;
                else if (this is Building building) return building.CompareTo(otherbuilding);
            }

            //Currently ordered by age in scene tree (should be last resort)
            return GetIndex().CompareTo(other.GetIndex());//TODO: Sort units by priority based on their "Heroicness" then the number of abilities, then I guess their cost.

        }
        public Godot.Collections.Array actions;
    }
}
