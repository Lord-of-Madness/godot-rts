using Godot;
using RTS.Gameplay;
using RTS.scripts.Gameplay;
using System;

namespace RTS.Gameplay
{
    public partial class UnitActions : GridContainer
    {
        [Export]
        int rows = 4;
        int BUTTON_COUNT { get => rows * Columns; }
        public void DestroyChildren()
        {
            foreach (var ability in GetChildren())//Clean up the old buttons
            {
                RemoveChild(ability);
                ability.QueueFree();
            }
        }

        public void FillGridButtons(Godot.Collections.Dictionary<int, Ability> abilities, HumanPlayer player)
        {
            DestroyChildren();
            for (int i = 0; i < BUTTON_COUNT; i++)
            {
                AbilityButton button = new();//This feels wasteful but safer. I know not of a way to otherwise clean up the buttons safely.
                AddChild(button);//Needs to be here cause _Ready gets called here and this is when we need it to happen
                if (abilities.TryGetValue(i, out Ability ability))
                {
                    button.Ability = ability;
                    if (ability.Targeted) {
                        button.Pressed += () => {
                            player.Clickmode = Player.ClickMode.UseAbility;
                            //TODO: player.ActiveAbility = ability ???
                            //We have to give the Selectable in charge of this Ability the order to perform it. Through the player somehow. Also handle things like the Unit's death etc. 
                        };
                    }

                }

            }
        }
    }
}