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

        public Unit unit;
        public override void _Ready()
        {
            base._Ready();
            Text = "Train " + unit.Name;
        }
        public TrainAbility(Unit unit)
        {
            this.unit = unit;
        }
        public override void OnClick(AbilityButton button)
        {
            base.OnClick(button);
            //TODO: Training time
            Unit newUnit = (Unit)unit.Duplicate();
            //GD.Print(OwningSelectable.Name);
            //GD.Print(newUnit.Name + " + " + newUnit.GetType());
            newUnit.Position = OwningSelectable.Position;
            OwningSelectable.AddSibling(newUnit);

            if (OwningSelectable is Building ownBuilding)
            {
                //GD.Print("Building");
                newUnit.Command(Player.ClickMode.Move, ownBuilding.RallyPoint);
            }
            else if (OwningSelectable is Unit unit)
            {
                newUnit.Command(Player.ClickMode.Move, unit.target);
                //Might wanna use the same ClickMode as unit but Clickmode and SelectableAction aren't the same tho if we ever Change SelectableAction to Actions then it will be simpler
            }
        }
    }
}