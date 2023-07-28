using Godot;
using RtsZápočťák.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtsZápočťák.Gameplay
{
    public partial class Selectable : CharacterBody2D, IComparable<Selectable>
    {
        public UnitGraphics Graphics;
        /// <summary>
		/// Graphicaly shows the selected status.
		/// </summary>
		public void Select()
        {
            Graphics.Select();
        }
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
