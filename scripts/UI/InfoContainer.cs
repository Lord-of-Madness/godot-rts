using Godot;
using RTS.Gameplay;
using System;
using System.Collections.Generic;

namespace RTS.UI;
public partial class InfoContainer : TabContainer
{
    UnitsSelected UnitsSelected;
    UnitInfo UnitInfo;
    //TODO: figure out / bind to Keybind tabing through the two tabs
    public void Update(SortedSet<Selectable> Selection,Selectable highlighted)
    {
        UnitsSelected.Update(Selection);
        UnitInfo.Update(highlighted);

    }
    public override void _Ready()
    {
        base._Ready();
        UnitsSelected = GetNode<UnitsSelected>(nameof(UnitsSelected));
        UnitInfo = GetNode<UnitInfo>(nameof(UnitInfo));
    }
}
