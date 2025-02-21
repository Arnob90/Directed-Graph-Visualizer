using Godot;
using System;
using CircleSpace;
namespace CircleSpace;
[Tool]
public partial class CircularLineTest : Node2D
{
    [Export]
    public CircleLine Circle;
    float _Radius = 100;
    float _XStep = 0.05f;
    float _StrokeWidth = 5;
    [Export]
    float XStep
    {
        set
        {
            _XStep = value;
            CallDeferred(MethodName.RefreshCircleProperties);
        }
        get => _XStep;
    }
    [Export]
    float Radius
    {
        set
        {
            _Radius = value;
            CallDeferred(MethodName.RefreshCircleProperties);
        }
        get => _Radius;
    }
    [Export]
    float StrokeWidth
    {
        set
        {
            _StrokeWidth = value;
            CallDeferred(MethodName.RefreshCircleProperties);
        }
        get => _StrokeWidth;
    }
    public override void _Ready()
    {
        RefreshCircleProperties();
    }
    private void RefreshCircleProperties()
    {
        Circle.Radius = Radius;
        Circle.XStep = XStep;
        Circle.Width = StrokeWidth;
    }
}
