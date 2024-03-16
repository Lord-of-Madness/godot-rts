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
    public void Update(Selection Selection)
    {
        UnitsSelected.Update(Selection);
        UnitInfo.Update(Selection.highlightedSelectable);

    }
    public override void _Ready()
    {
        base._Ready();
        UnitsSelected = GetNode<UnitsSelected>(nameof(UnitsSelected));
        UnitInfo = GetNode<UnitInfo>(nameof(UnitInfo));
    }
}
