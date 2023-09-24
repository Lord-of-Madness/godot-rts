using Godot;
using RTS.Graphics;
using RTS.scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Gameplay
{
    public static class TeamExtension
    {
        public static bool IsHostile(this Selectable.Team team)
        {
            return false;
        }
    }
    public partial class Selectable : CharacterBody2D, IComparable<Selectable>
    {
        [Signal] public delegate void SignalDisablingSelectionEventHandler(Unit unit);//when dead, loss of control etc.
        public enum Team
        {

        }

        public UnitGraphics Graphics;
        public Team team;
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
        public override void _Ready()
        {
            Graphics = GetNode<UnitGraphics>("Graphics");
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
