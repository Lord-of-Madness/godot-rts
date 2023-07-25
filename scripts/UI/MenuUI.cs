using Godot;
using RTSmainspace;
using System;

namespace RTSUI
{
    public partial class MenuUI : MenuButton
    {

#pragma warning disable IDE1006 // Naming Styles
        private void _on_Menu_toggled(bool toggleOn)
#pragma warning restore IDE1006 // Naming Styles
        {
            ((Player)Owner).TogglePause(toggleOn);
        }
        public override void _Ready()
        {
            base._Ready();
            var popup = GetPopup();
            popup.IdPressed += Popup_IdPressed;
        }
        enum Buttons
        {
            Save,
            Load,
            Options,
            ExitToMenu,
            ExitGame
        }

        private void Popup_IdPressed(long id)
        {
            switch ((Buttons)id)
            {
                case Buttons.Save:
                    break;
                case Buttons.Load:
                    break;
                case Buttons.Options:
                    break;
                case Buttons.ExitToMenu:
                    break;
                case Buttons.ExitGame:
                    GetTree().Quit();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}