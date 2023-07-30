using Godot;

namespace RTS.mainspace
{
    public interface IDamagable
    {
        public int HP { get; set; }
        //[Signal] public delegate void DeadEventHandler();
        //[Signal] public delegate void DamagedEventHandler();
        public void HealthChanged();
        public void Dead();
        public void Damaged();
    }
}
