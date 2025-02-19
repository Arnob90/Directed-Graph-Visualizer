using Godot;
using System;
using CircleSpace;
namespace CircleSpace;
[Tool]
public partial class CircularLineTest : Node2D
{
    public CircleLine Circle;
    float _Radius = 100;
    float _XStep = 0.05f;
    [Export]
    float XStep
    {
        set
        {
            _XStep = value;
            if (Circle != null && Circle.IsNodeReady())
            {
                Circle.Radius = Radius;
            }
        }
        get => _XStep;
    }
    [Export]
    float Radius
    {
        set
        {
            _Radius = value;
            if (Circle != null && Circle.IsNodeReady())
            {
                Circle.Radius = Radius;
            }
        }
        get => _Radius;
    }
    public override void _Ready()
    {
        Circle = GetNode<CircleLine>("%CircleLine");
        Circle.Radius = Radius;
        Circle.XStep = XStep;
    }
}
