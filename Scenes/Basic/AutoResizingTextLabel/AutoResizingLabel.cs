using Godot;
using System;

public partial class AutoResizingLabel : Control
{
	[Export]
	public Label MainLabel;
	[Export]
	public double WordsRequiredToFitIn100Px=50;
	public override void _Ready()
	{
	}
	private void CalculateAndUpdateSize()
	{
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
