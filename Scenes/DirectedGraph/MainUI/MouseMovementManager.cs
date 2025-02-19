using Godot;
using System;
using MouseMovementStatusSpace;
public partial class MouseMovementManager : Node2D
{
    [Export]
    public Camera2D CameraToManage;
    MouseStatus CurrentStatus;
    [Export]
    Label PositionLabel;
    public override void _Ready()
    {
        CurrentStatus = new Rest();
    }
    public override void _Process(double delta)
    {
        if (CameraToManage != null && CameraToManage.IsNodeReady())
        {
            if (Input.IsActionJustPressed("MovementHold"))
            {
                CurrentStatus = new Moving(GetLocalMousePosition());
            }
            if (Input.IsActionJustReleased("MovementHold"))
            {
                CurrentStatus = new Rest();
            }
            if (CurrentStatus is Moving previousCameraStatus)
            {
                var currentMousePos = GetLocalMousePosition();
                var offset = currentMousePos - previousCameraStatus.CurrentMousePosition;
                //We must go the opposite way we are pulling the mouse
                CameraToManage.Position = CameraToManage.Position - offset;
                CurrentStatus = new Moving(currentMousePos);
            }
            PositionLabel.Text = CameraToManage.Position.ToString();
        }
    }
}
