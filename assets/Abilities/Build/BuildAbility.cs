using Godot;
using Godot.Collections;

//using MonoCustomResourceRegistry;
using RTS.Gameplay;
using RTS.scripts.Gameplay;
using System;
namespace RTS.Gameplay
{
    public partial class BuildBuildingAbility : Ability
    {
        private string text = "Build _missing_";
        public override string Text { get => text; set =>text=value; }

        public override double Cooldown => 0f;

        public override bool Targeted => true;
        public Building building;
        public BuildBuildingAbility(Building building)
        {
            this.building = building;
            this.Text = "Build " + building.Name;
        }
        public override void OnClick(Button button)
        {
            base.OnClick(button);
            GD.Print(Text);
        }
    }
    //    [RegisteredType(nameof(BuildAbility),"",nameof(Resource))]//
    [GlobalClass]
    public partial class BuildAbility : Ability
    {
        public override void OnClick(Button button)
        {
            base.OnClick(button);
            UnitActions original = button.GetParent<UnitActions>();
            //UnitActions gc = (UnitActions)original.Duplicate();

            Dictionary<int, Ability> abilities = new();
            for (int i =0;i< buildings.Count;i++)
            {
                Building item = buildings[i];
                BuildBuildingAbility ability = new(item);
                var b = new AbilityButton()
                {
                    Ability = ability
                };
                AddChild(ability);
                //gc.AddChild(b);
                
                abilities.Add(i, ability);
            }
            original.FillGridButtons(abilities, original.GetTree().CurrentScene.GetNode<HumanPlayer>("Player"));
            //original.Visible = false;
            //original.GetParent().AddChild(gc);
        }
        [Export]
        public Array<BuildingBlueprint> BuildList { get; set; }
        private Array<Building> buildings = new();
        public override void _Ready()
        {
            base._Ready();
            foreach (var blueprint in BuildList)
            {
                GD.Print("Hmm");
                buildings.Add(blueprint.GetBuilding());
            }
        }

        public override double Cooldown => 0f;

        public override bool Targeted => false;

        public override string Text { get => "Build"; set => throw new Exception("This ought not be assigned to"); }
    }
}