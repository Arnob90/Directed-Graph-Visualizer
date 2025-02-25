using Godot;
using System;

public partial class CollaspeButton : TextureButton
{
	public override void _Ready()
	{
		CallDeferred(MethodName.SetDefaultRotation);
	}

	public override void _Process(double delta)
	{
	}
	private void SetDefaultRotation()
	{
		RotationDegrees = 90;
	}
}
