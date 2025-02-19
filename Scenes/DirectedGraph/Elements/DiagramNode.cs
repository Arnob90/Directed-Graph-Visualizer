using Godot;
using System;
namespace DiagramNodeSpace;
[Tool]
public partial class DiagramNode : Node2D
{
    [Export]
    public Label TextLabel;
    [Export]
    Container MainContainer;
    [Export]
    public Marker2D UpAnchor;
    [Export]
    public Marker2D DownAnchor;
    [Export]
    public Marker2D LeftAnchor;
    [Export]
    public Marker2D RightAnchor;
    [Export]
    public Marker2D CenterAnchor;
    public Vector2 TotalSize { get => MainContainer.Size; }
}
