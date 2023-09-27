using RTS.Gameplay;
using RTS.mainspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace RTS.Gameplay
{
    public partial class Building : Damageable, IComparable<Building>
    {

        public Vector2 RallyPoint;
        Line2D RallyPath;//TODO: update when reset (use the navagent we stole from unit)
        public int CompareTo(Building other)
        {
            return GetIndex().CompareTo(other.GetIndex());//For now
        }

        public override void Dead()
        {
            EmitSignal(SignalName.SignalDisablingSelection, this);
            EmitSignal(SignalName.SignalDead);
            Graphics.DeathAnim();//At the end it will remove the unit
        }
    }
}
