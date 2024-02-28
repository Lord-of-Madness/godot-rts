using Godot;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class BuildBuilidngAbilityRes : AbilityRes
    {
        public override string Text { get; set; }//This does nothing on purpose (The node itself has logic on how to do it)

        [Export]
        public PackedScene Building;
        public override BuildBuildingAbility Instantiate(Selectable owner)
        {
            return new(Building.Instantiate<Building>()) {OwningSelectable = owner };
        }
    }
}