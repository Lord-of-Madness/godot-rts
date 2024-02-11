using Godot;
using System;

namespace RTS.Gameplay
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
        //[ExportGroup("CombatStats")]
        [Export] public int MaxHP { get; set; }
        protected ProgressBar HealthBar;
        public void HealthChanged()
        {
            //GD.Print(HP, " ", Name);
            HealthBar.Value = Math.Max(0, HP);
            EmitSignal(SignalName.SignalHealthChanged);
            if (HP <= 0) Dead();
        }
        public abstract void Dead();
        public void Damaged()
        {
            EmitSignal(SignalName.SignalDamaged);
        }
        public override void _Ready()
        {
            base._Ready();
            HealthBar = Graphics.GetNode<ProgressBar>(nameof(HealthBar));
            HealthBar.MaxValue = MaxHP;
            HP = MaxHP;
        }
    }
}
