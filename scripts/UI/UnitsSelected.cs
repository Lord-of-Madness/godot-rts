using Godot;
using RTS.Gameplay;
using System;
using System.Linq;
using RTS.mainspace;
using System.Collections.Generic;

namespace RTS.UI
{
    public partial class UnitFaceButton : Button
    {
        public Selectable Selectable;
        public UnitFaceButton(Selectable selectable)
        {
            ExpandIcon = true;
            VerticalIconAlignment = VerticalAlignment.Center;
            SizeFlagsHorizontal = SizeFlags.ExpandFill;
            SizeFlagsVertical = SizeFlags.ExpandFill;
            Selectable = selectable;
            Icon = selectable.GetNode<Sprite2D>("UnitPortrait").Texture;
        }
    }
    public partial class UnitsSelected : GridContainer
    {

        public void Update(Selection Selection)
        {
            this.DestroyChildren();

            var suEnum = Selection.GetEnumerator();
            while (suEnum.MoveNext())
            {
                if (suEnum.Current.CurrentAction == Selectable.SelectableAction.Dying) continue;//I have to do this smart and not put pieces of ducttape all over the place
                //TODO: if we overshoot certain ammount of rows we should make tabs for the rest of the selected units
                //this feels like the best way to do it while keeping the sortedSet. (Eventually could change it to SortedList I guess and just index into it)
                //TODO:display Health/other bars underneath (TextureRect will not be enough)
                UnitFaceButton button = new(suEnum.Current);
                button.ButtonDown += () =>
                {
                    Selection.highlightedSelectable = button.Selectable;//TODO this is just partly done (gotta put the control over unit portrait to the InfoContainer from HumanPlayer)
                    GD.Print(button.Selectable.SName);
                };
                AddChild(button);
                /*AddChild(new TextureRect()
                {
                    ExpandMode = TextureRect.ExpandModeEnum.FitHeight,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                    SizeFlagsHorizontal = SizeFlags.ExpandFill,
                    SizeFlagsVertical = SizeFlags.ExpandFill,
                    Texture = suEnum.Current.GetNode<Sprite2D>("UnitPortrait").Texture
                });*/

            }
        }

    }
}
