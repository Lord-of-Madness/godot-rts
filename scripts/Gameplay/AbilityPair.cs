using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class AbilityPair : Resource//Dictionaries export badly, also has to be a Resource to be exportable
    {
        [Export]
        public int pos;
        [Export]
        public PackedScene ability;
    }
}
