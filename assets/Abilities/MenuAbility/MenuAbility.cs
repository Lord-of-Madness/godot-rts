using Godot;
using Godot.Collections;
using RTS.Physics;
using System;
namespace RTS.Gameplay
{
    /// <summary>
    /// Works as a "Back" button for MenuAbilities
    /// </summary>
    public partial class BackAbility : Ability
    {
        Dictionary<ushort, Ability> PreviousAbilityList;
        public BackAbility(Dictionary<ushort, Ability> previousAbList, Selectable owner)
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
    /// <summary>
    /// Acts like a folder in the Ability UI to group things like Buildings together
    /// </summary>
    public partial class MenuAbility : Ability
    {
        [Export]
        public override string Text { get; set; }
        public override bool Active => true;
        public override Second Cooldown => new(0f);

        [Export]
        public Dictionary<ushort, Ability> abilities;

        public MenuAbility(Dictionary<ushort, Ability> abilities , Selectable owner,string text)
        {
            this.abilities = abilities;
            Text = text;
            OwningSelectable = owner;
            foreach (var pair in this.abilities)
            {
                AddChild(pair.Value);
            }


        }

        public override void OnClick(AbilityButton button)
        {
            base.OnClick(button);
            UnitActions original = button.GetParent<UnitActions>();
            if (!abilities.ContainsKey((ushort)(original.BUTTON_COUNT - 1u)))
            {
                BackAbility backAbility = new(OwningSelectable.Abilities, OwningSelectable);//This is duplicated on purpose cause if there are multiple levels of Lists the Owning Selectable is still the same but Ability list might be different
                AddChild(backAbility);
                abilities.Add((ushort)(original.BUTTON_COUNT - 1), backAbility);
            }
            original.FillGridButtons(abilities);
        }

    }
}