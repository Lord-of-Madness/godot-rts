using Godot;
using RTS.UI;
using System;

namespace RTS.Gameplay {
    public partial class GameResourceSource : Selectable
    {
        public override void Command(Player.ClickMode clickMode, Target target, Ability ability = null)
        {
            GD.Print("No point giving this guy any commands");
        }
        [Export(PropertyHint.Range,"0,1000,or_greater")]
        public int Capacity = 0;



    }
}