using Godot;
using RTS.mainspace;
using System;

namespace RTS.UI;
public partial class Minimap : SubViewportContainer
{
	SubViewport viewport;
	public override void _Ready()
	{
		viewport = GetNode<SubViewport>(nameof(SubViewport));
		viewport.World2D = GetViewport().World2D;
		viewport.GetCamera2D().Zoom = new(viewport.Size.X / GetViewport().GetCamera2D().LimitWidth(), viewport.Size.Y/ GetViewport().GetCamera2D().LimitHeight());
	}
}
