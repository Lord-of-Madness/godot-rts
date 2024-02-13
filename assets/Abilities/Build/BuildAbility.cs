using Godot;
using Godot.Collections;
//using MonoCustomResourceRegistry;
using RTS.Gameplay;
using RTS.Physics;
using RTS.scripts.Gameplay;
using System;
namespace RTS.Gameplay
{

    public partial class BuildBuildingAbility : TargetedAbility
    {
        private string text = "Build _missing_";
        public override string Text { get => text; set =>text=value; }

        public override Second Cooldown => new(0f);

        public override bool Active => true;

        public Building building;
        public BuildBuildingAbility(Building building, Selectable owner)
        {
            OwningSelectable = owner;
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
            OwningSelectable.AddSibling(newBuilding);
        }
    }
    public partial class BackAbility : Ability
    {
        public BackAbility(Selectable owner){
            OwningSelectable = owner;
        }

        public override string Text { get => "Back"; set => throw new Exception("This ought not be assigned to"); }

        public override Second Cooldown => 0;

        public override bool Active => true;

        public override void OnClick(AbilityButton button)
        {
            base.OnClick(button);
            button.GetParent<UnitActions>().FillGridButtons(OwningSelectable.Abilities);

        }
    }
    //    [RegisteredType(nameof(BuildAbility),"",nameof(Resource))]//
    [GlobalClass]
    public partial class BuildAbility : Ability
    {
        public override void OnClick(AbilityButton button)
        {
            base.OnClick(button);
            UnitActions original = button.GetParent<UnitActions>();

            Dictionary<int, Ability> abilities = new();
            for (int i =0;i< buildings.Count;i++)
            {
                Building item = buildings[i];
                BuildBuildingAbility ability = new(item, OwningSelectable);
                AddChild(ability);
                abilities.Add(i, ability);
            }
            BackAbility backAbility = new(OwningSelectable);
            AddChild(backAbility);
            abilities.Add(original.BUTTON_COUNT - 1, backAbility);
            original.FillGridButtons(abilities);
        }
        [Export]
        public Array<BuildingBlueprint> BuildList { get; set; }
        private Array<Building> buildings = new();

        public override void _Ready()
        {
            base._Ready();
            foreach (var blueprint in BuildList)
            {
                buildings.Add(blueprint.Building);
            }
        }

        public override Second Cooldown => new(0f);

        public override string Text { get => "Build"; set => throw new Exception("This ought not be assigned to"); }

        public override bool Active => true;
    }
}