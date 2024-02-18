using Godot;
//using MonoCustomResourceRegistry;
using RTS.Physics;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class BuildBuildingAbility : TargetedAbility
    {
        private string text = "Build _missing_";
        public override string Text { get => text; set =>text=value; }

        public override Second Cooldown => new(0f);

        public override bool Active => true;

        public Building building;
        public BuildBuildingAbility(Building building)
        {
            this.building = building;
            Text = "Build " + building.Name;
        }
        public override void OnClick(AbilityButton button)
        {
            base.OnClick(button);
            GD.Print("TODO: Display building wireframe at the mouseposition");
        }

        public override void OnTargetRecieved(Target target)
        {
            Building newBuilding = (Building)building.Duplicate();
            newBuilding.Position = target.Position;
            if (OwningSelectable is null) GD.Print("OnTargetRecieved");
            OwningSelectable.AddSibling(newBuilding);
        }
    }
}