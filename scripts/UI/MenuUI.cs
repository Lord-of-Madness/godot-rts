using Godot;
using System;

public class MenuUI : MenuButton
{
    private void _on_Menu_toggled(bool toggleOn)
    {
        ((GameLevel)Owner).TogglePause(toggleOn);
    }
    
}
