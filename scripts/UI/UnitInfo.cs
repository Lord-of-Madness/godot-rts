using Godot;
using RTS.Gameplay;
using RTS.mainspace;
using System;

namespace RTS.UI;
public partial class UnitInfo : Control
{
    Label NameLabel;
    Label HPLabel;
    GridContainer AttacksInfo;
    public override void _Ready()
    {
        NameLabel = GetNode<Label>(nameof(NameLabel));
        HPLabel = GetNode<Label>(nameof(HPLabel));
        AttacksInfo = GetNode<GridContainer>(nameof(AttacksInfo));
    }
    public void Update(Selectable selectable)
    {
        AttacksInfo.DestroyChildren();
        NameLabel.Text = "";
        HPLabel.Text = "";
        if (selectable is not null)
        {
            NameLabel.Text = selectable.SName;
            if (selectable is Damageable d) HPLabel.Text = d.HP.ToString();

            foreach (Attack attack in selectable.Attacks)
            {
                AttacksInfo.AddChild(new Label { Text = attack.Name, });
            }
        }
    }

}
