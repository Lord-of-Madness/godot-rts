using Godot;

namespace RTS.Gameplay
{
    public partial class AbilityButton : Button
    {
        //TODO:
        //Visual effect of the abilities
        //reference to the ability (so that we can see cooldowns and stuff)
        private Ability ability;
        public Ability Ability
        {
            get => ability;
            set
            {
                ability = value;
                Pressed += () => ability.OnClick(this);
                Icon = ability.Icon;
                Disabled = false;
                Text = ability.Text;
            }
        }
        public override void _Ready()
        {
            base._Ready();
            SizeFlagsHorizontal = SizeFlags.ExpandFill;
            SizeFlagsVertical = SizeFlags.ExpandFill;
            Disabled = true;
        }
    }
}

