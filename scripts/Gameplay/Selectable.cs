using Godot;
using RTS.Graphics;
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
        public int CompareTo(Selectable other)
        {
            //Currently ordered by age in scene tree (should be last resort)
            return GetIndex().CompareTo(other.GetIndex());//TODO: Sort units by priority based on their "Heroicness" then the number of abilities, then I guess their cost.

        }
        public Godot.Collections.Array actions;
    }
}
