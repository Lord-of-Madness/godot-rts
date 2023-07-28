using Godot;
namespace RtsZ·poËù·k.Gameplay
{
    [GlobalClass]
    public partial class Attack : Resource
    {
        public enum DamageType//not gonna use it now
        {
            Standart
        }
        [Export(PropertyHint.Range, "0,10,1,or_greater")] public float AttackSpeed { get; set; }
        [Export (PropertyHint.Range, "0,10,1,or_greater")] public float Damage { get; set; }
        [Export] public bool Ranged { get; set; }
        [Export] public DamageType Damagetype {get;set;}

    }
}

