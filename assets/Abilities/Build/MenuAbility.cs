using Godot;
using Godot.Collections;
using RTS.Physics;
using System;
namespace RTS.Gameplay
{
    public partial class BackAbility : Ability
    {
        Dictionary<int, Ability> PreviousAbilityList;
        public BackAbility(Dictionary<int, Ability> previousAbList, Selectable owner)
        {
            PreviousAbilityList = previousAbList;
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

    public partial class MenuAbility : Ability
    {
        public override string Text { get; set; }
        public override bool Active => true;
        public override Second Cooldown => new(0f);

        private Dictionary<int, Ability> abilities;

        public MenuAbility(Dictionary<int, Ability> abilities , Selectable owner)
        {
            this.abilities = abilities;
            OwningSelectable = owner;
            foreach (var pair in this.abilities)
            {
                if( OwningSelectable is null)GD.Print("MenuAbility");
                //T item = abilities[i];
                //BuildBuildingAbility ability = new(item, OwningSelectable);//Rather than the building itself I should add an ability to build said building in the BuildAbility
                pair.Value.OwningSelectable = OwningSelectable;
                AddChild(pair.Value);
                //abilities.Add(i, pair.Value);
            }


        }

        //public Array<Blueprint<T>> SceneList { get; set; }
        public override void OnClick(AbilityButton button)
        {
            base.OnClick(button);
            UnitActions original = button.GetParent<UnitActions>();

            //Dictionary<int, Ability> abilities = new();

            if (!abilities.ContainsKey(original.BUTTON_COUNT - 1))
            {
                BackAbility backAbility = new(OwningSelectable.Abilities, OwningSelectable);//This is duplicated on purpose cause if there are multiple levels of Lists the Owning Selectable is still the same but Ability list might be different
                AddChild(backAbility);
                abilities.Add(original.BUTTON_COUNT - 1, backAbility);
            }
            original.FillGridButtons(abilities);
        }

    }
}