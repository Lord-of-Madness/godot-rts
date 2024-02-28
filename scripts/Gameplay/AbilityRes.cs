using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]//Will throw errors in Editor but it must be here so those who inherit from this can use it
    public abstract partial class AbilityRes : Resource
    {
        public abstract string Text { get; set; }
        public abstract Ability Instantiate(Selectable owner);
    }
}

