using Godot;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class BuildBuilidngAbilityRes : TargetedAbilityRes
    {
        public override string Text { get; set; }//This does nothing on purpose (The node itself has logic on how to do it)
        public override float Range { get; set; } = 0;

        [Export]
        public PackedScene Building;
        public override BuildBuildingAbility Instantiate(Selectable owner)
            => new(Building.Instantiate<Building>()) {OwningSelectable = owner };
        
    }
}