using Godot;
using RtsZ·poËù·k.Physics;
namespace RtsZ·poËù·k.Gameplay
{
    public partial class Map : Node2D
    {
        [Export(PropertyHint.Range, "0.1,10,1,or_greater")]
        public float Gamespeed = 1;

        [Export] bool fogofwar;
        public override void _Ready()
        {
            TilesPerSecond.physicsValues = new() { Gamespeed = Gamespeed };
            Persec.physicsValues = new() { Gamespeed = Gamespeed };
            if (fogofwar)
            {

                //Initialize the foggy overlay.(for each player seperately)

            }
        }

    }
}
