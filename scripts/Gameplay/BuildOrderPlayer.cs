using Godot;
using System;
using System.Linq;

namespace RTS.Gameplay
{
    public partial class BuildOrderPlayer : Player
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();

        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            base._Process(delta);
            foreach (Selectable item in GetNode("Selectables").GetChildren().Cast<Selectable>())
            {
                if(item is Building building)
                {
                    foreach (Ability ability in from pair in building.Abilities select pair.Value)
                    {
                        ability.OnUse();
                    }
                }
            }
        }
    }
}