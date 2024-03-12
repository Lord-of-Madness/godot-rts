using Godot;
using RTS.Gameplay;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RTS.UI
{

    public partial class UnitsSelected : GridContainer
    {
        public partial class InfoLabel : Label
        {
            public InfoLabel(string text)
            {
                Text = text;
            }
            public InfoLabel()
            {
                SizeFlagsHorizontal = SizeFlags.Fill;
                SizeFlagsVertical = SizeFlags.Fill;
            }
        }
        public void DestroyChildren()
        {
            foreach (var item in GetChildren())
            {
                RemoveChild(item);
                item.QueueFree();
            }
        }
        public void Update(SortedSet<Selectable> Selection)
        {
            DestroyChildren();
            if (Selection.Count == 1)
            {
                //Display stats about the Unit
                Selectable s = Selection.First();
                AddChild(new InfoLabel(s.Name));
                AddChild(new InfoLabel(s.team.ToString()));
                AddChild(new InfoLabel(s.CurrentAction.ToString()));

                if (s is Damageable d)
                {
                    AddChild(new InfoLabel("MaxHP: " + d.MaxHP));
                }
                if (s is Unit u)
                {
                    foreach (var item in u.Attacks)
                    {
                        AddChild(new InfoLabel("Attack [Name: " + item.Name + ", AoE: " + item.AoE + ", AttackPeriod: " + item.AttackPeriod + "s"));
                    }

                }
            }
            else
            {
                var suEnum = Selection.GetEnumerator();
                while (suEnum.MoveNext())
                {
                    //TODO: if we overshoot certain ammount of rows we should make tabs for the rest of the selected units
                    //this feels like the best way to do it while keeping the sortedSet. (Eventually could change it to SortedList I guess and just index into it)
                    //TODO:display Health/other bars underneath (TextureRect will not be enough)
                    AddChild(new TextureRect()
                    {
                        ExpandMode = TextureRect.ExpandModeEnum.FitHeight,
                        StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                        SizeFlagsHorizontal = SizeFlags.ExpandFill,
                        SizeFlagsVertical = SizeFlags.ExpandFill,
                        Texture = suEnum.Current.GetNode<Sprite2D>("UnitPortrait").Texture
                    });
                }
            }
        }

    }
}
