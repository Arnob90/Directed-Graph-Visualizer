using Godot;
using System;
namespace ResultLabelUISpace;
[Tool]
public partial class FailureLabelContainer : PanelContainer
{
	[Export]
	public Label FailureLabel;
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
