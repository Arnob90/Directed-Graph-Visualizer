using Godot;
using System;
using CircleSpace;
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
    [Export]
    private CircularLineTest CircularLine;
    public Vector2 TotalSize { get => MainContainer.Size; }
    public bool _MappedToSelf = false;
    [Export]
    public bool MappedToSelf { get => _MappedToSelf; set { _MappedToSelf = value; CallDeferred(MethodName.RefreshProperties); } }
    public override void _Ready()
    {
        RefreshProperties();
    }
    public void RefreshProperties()
    {
        CircularLine.Visible = MappedToSelf;
    }
}
