using Godot;
//using MonoCustomResourceRegistry;
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
            if (owner is null) GD.Print("BuildBuildingAbility Instantiate");
            return new BuildBuildingAbility(Building.Instantiate<Building>()) {OwningSelectable = owner };
        }
    }
}