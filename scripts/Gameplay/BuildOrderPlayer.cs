using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace RTS.Gameplay
{
    public partial class BuildOrderPlayer : Player
    {
        Selectable target = null;
        public override void _Process(double delta)
        {

            base._Process(delta);
            if (target is null)
            {
                var enemies = from sel in
                                  from c in GetParent().GetChildren() where c is Player p && p.Team != Team select c
                              select sel.GetNode("Selectables").GetChildren().Cast<Selectable>();
                Godot.Collections.Array enemies1d = new();
                foreach (var selectable in enemies)
                {
                    enemies1d.AddRange(selectable);
                }
                if (enemies1d.Count > 0)
                    target = (Selectable)enemies1d.First();
            }
            foreach (Selectable item in GetNode("Selectables").GetChildren().Cast<Selectable>())
            {
                if (item is Building building)
                {
                    if (target is not null) building.RallyPoint = new(target);
                    foreach (Ability ability in from pair in building.Abilities select pair.Value)
                    {
                        if (ability is not TargetedAbility)//just so we accidentaly don't do something weird
                            ability.BaseOnUse();
                    }
                }
                else if (item is Unit unit)
                {
                    if(unit.CurrentAction!= Selectable.SelectableAction.Attack)
                    unit.Command(ClickMode.Attack, new(target));
                }
            }
        }
    }
}