using Godot;
using System;

namespace RTS.Gameplay
{
    [GlobalClass]
    public abstract partial class Ability : Node
    {
        private double cooldown = 0;
        public virtual void OnClick(Button button)
        {
            if (onCooldown) return;
            if (Cooldown != 0)
            {
                cooldown = Cooldown;
                onCooldown = true;
            }

        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            cooldown -= delta;
            if (cooldown <= 0)
            {
                cooldown = 0;
                onCooldown = false;
            }
        }

        public Texture2D Icon;//TODO: Need to get it from somewhere also should probably set it as abstract too so it has to be set

        public abstract string Text { get; set; }//So everyone has to implement it
        private Key shortcut = Key.None;
        public Key Shortcut { get => shortcut; set => shortcut = value; }
        public abstract double Cooldown { get; }
        public abstract bool Targeted { get; }//I will now assume there are no other options but targeted and nontargeted
        public bool onCooldown = false;
    }
}

