using Godot;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class TrainAbilityRes : AbilityRes
    {
        public override string Text { get; set; }

        [Export]
        public PackedScene unit;
        public override TrainAbility Instantiate(Selectable owner)
        {
            return new(unit.Instantiate<Unit>()) { OwningSelectable = owner };
        }
    }
}