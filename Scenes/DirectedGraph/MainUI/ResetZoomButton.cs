using Godot;
using System;

public partial class ResetZoomButton : TextureButton
{
	[Export]
	Camera2D CameraToResetZoomOf;
	public override void _Ready()
	{
		Pressed+=ResetZoom;
	}

	private void ResetZoom()
	{
		CameraToResetZoomOf.Zoom=new Vector2(1,1);
	}
}
