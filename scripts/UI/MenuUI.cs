using Godot;
using System;

public partial class MenuUI : MenuButton
{
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
    private void _on_Menu_toggled(bool toggleOn)
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
    {
        ((GameLevel)Owner).TogglePause(toggleOn);
    }
    
}
