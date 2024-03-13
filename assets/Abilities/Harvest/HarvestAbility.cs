using Godot;
using RTS.Physics;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class HarvestAbility : TargetedAbility
    {
        public override string Text { get => "Harvest"; set => throw new System.NotImplementedException(); }
        public override Second Cooldown => 0;

        public override bool Active => true;

        GameResourceSource GameResourceSource { get; set; }
        //Building returnPoint { get; set; } Lets not put it here. It is better we just say: RETURN and the unit will find a way

        public override void OnTargetRecieved(Target target)
        {
            if (target.type == Target.Type.Selectable && target.selectable is GameResourceSource gameResource)
            {
                GameResourceSource = gameResource;
                //find return point
            }
        }

        protected override bool IsTarget(Node2D shape)
        {
            return (shape is GameResourceSource grs && grs.Equals(GameResourceSource));
        }
        public override void OnTargetReached()
        {
            base.OnTargetReached();
            GD.Print("Not implemented Harvesting action");
            //We should now
            //1. Give Unit thing to carry -> any unit is technicaly suitable if it has this ability
            //2. Give the unit new command to return to return point
        }

        public override void OnUse(){}
    }
}
