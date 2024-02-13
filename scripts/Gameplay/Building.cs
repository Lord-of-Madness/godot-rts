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
    [GlobalClass]
    public partial class Building : Damageable, IComparable<Building>
    {

        public Target RallyPoint;
        private bool following = false;
        Line2D RallyPath;//TODO: update when reset (use the navagent we stole from unit)

        public override void CleanCommandQueue()
        {
            return;//Buildings don't have commmand queues
            //TODO: Ponder moving CleanCommandQueue only to Unit away from Selectables
        }

        public override void Command(Player.ClickMode clickMode, Target target, Ability ability = null)
        {
            switch (clickMode)
            {
                case Player.ClickMode.Move:
                    SetRally(target);
                    break;
            }
        }
        public void SetRally(Target target)
        {
            RallyPoint = target;
            if (target.type == Target.Type.Location)
            {
                NavAgent.TargetPosition = target.location;
            }
            else
            {
                following = true;
                if (target.selectable is Damageable damageable)//TODO: The damageable should perhaps be Selectable and it should deRally even on disapearing into the fog of war if its not our Selectable
                {
                    damageable.SignalDead += () => SetRally(new() { type = Target.Type.Location, location = damageable.Position });//This oughta mean that the Rally point stays where the unit died
                    //player.VisionArea.BodyExited += Detarget; //TODO when outside vision
                }
            }
        }

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
        /*public override void _Ready()
        {
            base._Ready();
        }*/
    }
}
