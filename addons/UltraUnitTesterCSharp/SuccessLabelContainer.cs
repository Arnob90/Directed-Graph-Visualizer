using Godot;
using System;
namespace ResultLabelUISpace;
[Tool]
public partial class SuccessLabelContainer : PanelContainer
{
	[Export]
	public Label SuccessLabel;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
