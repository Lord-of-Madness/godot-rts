using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]
    public abstract partial class AbilityRes : Resource
    {
        public abstract string Text { get; set; }
        public abstract Ability Instantiate(Selectable owner);
    }
}

