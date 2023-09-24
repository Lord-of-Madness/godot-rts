using Godot;
using RTS.Gameplay;
using System;

namespace RTS.mainspace
{
    public abstract partial class Damageable : Selectable 
    {
        [Signal] public delegate void SignalDeadEventHandler();
        [Signal] public delegate void SignalDamagedEventHandler();
        [Signal] public delegate void SignalHealthChangedEventHandler();
        private int hp;
        public int HP
        {
            get { return hp; }
            set
            {
                if (hp > value) Damaged();
                hp = value;
                HealthChanged();
            }
        }
        protected ProgressBar HealthBar;
        public void HealthChanged()
        {
            GD.Print(HP, " ", Name);
            HealthBar.Value = Math.Max(0, HP);
            GD.PrintErr(EmitSignal(SignalName.SignalHealthChanged));
            if (HP <= 0) Dead();
        }
        public abstract void Dead();
        public void Damaged()
        {
            EmitSignal(SignalName.SignalDamaged);
        }
    }
}
