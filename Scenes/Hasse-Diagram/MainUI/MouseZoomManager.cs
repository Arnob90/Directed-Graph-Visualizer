using Godot;
using System;

public partial class MouseZoomManager : Node2D
{
    [Export]
    Camera2D CameraToControlZoomOf;
    [Export]
    float ZoomMultiplier = 1.5f;
    [Export]
    Label ZoomLabel;
    [Export]
    float MinZoom = 0.001f;
    [Export]
    float MaxZoom = 1000f;
    public override void _Ready()
    {
    }
    private void ZoomCamera(float factor)
    {
        Vector2 previousZoom = CameraToControlZoomOf.Zoom;
        Vector2 zoom = CameraToControlZoomOf.Zoom * factor;
        Vector2 viewportCenterCoords = GetViewportRect().Size / 2;
        Vector2 mousePosInScreenSpaceFromCenter = GetViewport().GetMousePosition() - viewportCenterCoords;
        zoom = zoom.Clamp(MinZoom, MaxZoom);
        Vector2 postZoomPosOffset = new Vector2(mousePosInScreenSpaceFromCenter.X * ((1 / previousZoom.X) - (1 / zoom.X)), mousePosInScreenSpaceFromCenter.Y * ((1 / previousZoom.Y) - (1 / zoom.Y)));
        CameraToControlZoomOf.Position = CameraToControlZoomOf.Position + postZoomPosOffset;
        CameraToControlZoomOf.Zoom = zoom;
    }
    public override void _Process(double delta)
    {
        //We add safety checks to make sure it's safe to handle the given camera yet.
        if (CameraToControlZoomOf != null && CameraToControlZoomOf.IsNodeReady())
        {
            if (Input.IsActionJustPressed("Zoom In"))
            {
                ZoomCamera(ZoomMultiplier);
            }
            if (Input.IsActionJustPressed("Zoom Out"))
            {
                ZoomCamera(1 / ZoomMultiplier);
            }
            ZoomLabel.Text = CameraToControlZoomOf.Zoom.ToString();
        }
    }
}
