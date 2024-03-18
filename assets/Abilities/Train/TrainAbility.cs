using Godot;
using RTS.Physics;
using System;
namespace RTS.Gameplay
{
    [GlobalClass]
    public partial class TrainAbility : Ability
    {
        public override string Text { get; set; }

        public override Second Cooldown => new(5f);

        public override bool Active => true;

        [Export]
        public float trainingTime;
        public Second TraingTime { get => trainingTime; }//TODO UI sauce and all that jazz


        public Unit unit;
        public override void _Ready()
        {
            base._Ready();
            Text = "Train " + unit.SName;
        }
        public TrainAbility(Unit unit)
        {
            this.unit = unit;
        }
        public override void OnUse()
        {
            //TODO: Training time
            //Building oughta have a production queue to which we add this.
            //So we should instead of spawnign a unit add a TrainJob to the Building and it should handle it itself
            Unit newUnit = (Unit)unit.Duplicate();
            TrainJob trainJob = new (newUnit);
            //TODO: OwningSelectable.BuildQueue.Add(trainJob)
            //The thing under us should be yeeted into unit/building themselves
            newUnit.Position = OwningSelectable.Position;
            OwningSelectable.AddSibling(newUnit);

            if (OwningSelectable is Building ownBuilding)
            {
                //GD.Print("Building");
                //GD.Print(ownBuilding.RallyPoint);
                if(ownBuilding.RallyPoint is Damageable d && d.team.IsHostile(ownBuilding.team))
                {
                    newUnit.Command(Player.ClickMode.Attack, ownBuilding.RallyPoint);
                }
                else
                    newUnit.Command(Player.ClickMode.Move, ownBuilding.RallyPoint);
            }
            else if (OwningSelectable is Unit unit)
            {
                newUnit.Command(Player.ClickMode.Move, unit.Target);
                //Might wanna use the same ClickMode as unit but Clickmode and SelectableAction aren't the same tho if we ever Change SelectableAction to Actions then it will be simpler
            }
        }
    }
    public struct TrainJob
    {
        public Second TraingTime { get; }
        public Unit unit;
        public TrainJob(Unit unit) {
            this.unit = unit;
        }
    }
}