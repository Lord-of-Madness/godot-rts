using Godot;
using RTS.Gameplay;
using System;
using System.Linq;
using RTS.mainspace;
using System.Collections.Generic;

namespace RTS.UI
{

    public partial class UnitsSelected : GridContainer
    {
        
        public void Update(SortedSet<Selectable> Selection)
        {
            this.DestroyChildren();

            var suEnum = Selection.GetEnumerator();
            while (suEnum.MoveNext())
            {
                //TODO: if we overshoot certain ammount of rows we should make tabs for the rest of the selected units
                //this feels like the best way to do it while keeping the sortedSet. (Eventually could change it to SortedList I guess and just index into it)
                //TODO:display Health/other bars underneath (TextureRect will not be enough)
                var button =
                new Button()
                {
                    ExpandIcon = true,
                    VerticalIconAlignment = VerticalAlignment.Center,
                    SizeFlagsHorizontal = SizeFlags.ExpandFill,
                    SizeFlagsVertical = SizeFlags.ExpandFill,
                    Icon = suEnum.Current.GetNode<Sprite2D>("UnitPortrait").Texture,
                };
                button.Pressed += () =>
                {
                    //Highlighted selectable
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
