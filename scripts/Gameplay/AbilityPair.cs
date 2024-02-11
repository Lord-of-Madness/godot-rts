using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class AbilityPair : Resource//Dictionaries export badly, also has to be a Resource to be exportable
    {
        [Export]
        public int pos;
        [Export]
        public Ability ability;
        /*
        public AbilityPair(int pos, Ability ability)
        {
            this.pos = pos;
            this.ability = ability;
        }*/
    }
}
