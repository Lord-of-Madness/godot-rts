using Godot;
using Godot.Collections;
using System.Linq;
using RTS.mainspace;
namespace RTS.Gameplay
{


    public partial class AttacksNode : Node2D
    {
        [Export]
        Attack PrimaryAttack;//TODO: Changeable by the player. Will change to which position will the unit try to get if it has more than 1 attack available.
        public Array<Attack> Attacks
        {
            get
            {
                return GetChildren().Cast<Attack>().ToGodotArray();
            }
        }
    }
}