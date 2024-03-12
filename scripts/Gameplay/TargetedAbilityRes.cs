using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]
    public abstract partial class TargetedAbilityRes : AbilityRes
    {
        [Export(PropertyHint.Range, "0,10,1,or_greater")]
        public abstract float Range { get; set; }

        public override abstract TargetedAbility Instantiate(Selectable owner);
    }
}

