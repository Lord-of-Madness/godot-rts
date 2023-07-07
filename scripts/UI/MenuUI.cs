using Godot;
using RTSmainspace;
namespace RTSUI
{
    public partial class MenuUI : MenuButton
    {
#pragma warning disable IDE1006 // Naming Styles
        private void _on_Menu_toggled(bool toggleOn)
#pragma warning restore IDE1006 // Naming Styles
        {
            ((Player)Owner).TogglePause(toggleOn);//This is pausing the player not the actual game.
        }

    }
}
