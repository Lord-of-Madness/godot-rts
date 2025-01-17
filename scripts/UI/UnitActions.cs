using Godot;

namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class UnitActions : GridContainer
    {
        [Export]
        ushort rows = 4;
        public ushort BUTTON_COUNT { get => (ushort)(rows * (ushort)Columns); }
        public void DestroyChildren()
        {
            foreach (var ability in GetChildren())//Clean up the old buttons
            {
                RemoveChild(ability);
                ability.QueueFree();
            }
        }
        HumanPlayer player;
        public override void _Ready()
        {
            base._Ready();
            player = GetParent().GetParent().GetParent().GetParent<HumanPlayer>();//Crude? Yes. Effective? Yes
        }

        public void FillGridButtons(Godot.Collections.Dictionary<ushort, Ability> abilities)
        {

            DestroyChildren();
            for (ushort i = 0; i < BUTTON_COUNT; i++)
            {

                AbilityButton button = new();//This feels wasteful but safer. I know not of a way to otherwise clean up the buttons safely.
                AddChild(button);//Needs to be here cause _Ready gets called here and this is when we need it to happen
                if (abilities.TryGetValue(i, out Ability ability))
                {
                    button.Ability = ability;
                    if (ability is TargetedAbility targetedAbility)
                    {
                        button.Pressed += () =>
                        {
                            player.Clickmode = Player.ClickMode.UseAbility;
                            player.HangingAbility = targetedAbility;
                            //We have to give the Selectable in charge of this Ability the order to perform it. Through the player somehow. Also handle things like the Unit's death etc. 
                        };
                    }

                }

            }
        }
    }
}