using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class AbilityPair : Resource//Dictionaries export badly, also has to be a Resource to be exportable
    {
        [Export(PropertyHint.Range,"0,23,1")]
        public ushort pos;
        [Export]
        public AbilityRes ability;
    }
}
