using Godot;
using RTS.Physics;
using System;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class TrainAbility : Ability
    {
        public override string Text { get => "Train"; set => throw new Exception("Ought not set this"); }

        public override Second Cooldown => new(5f);

        public override bool Active => true;

        public override void OnClick(AbilityButton button)
        {
            base.OnClick(button);
        }
    }
}