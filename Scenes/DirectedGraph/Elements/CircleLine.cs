using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
namespace CircleSpace;
[Tool]
public partial class CircleLine : Line2D
{
    float _Radius = 1;
    public float Radius
    {
        get => _Radius;
        set
        {
            _Radius = value;
            UpdateCircle();
        }
    }
    float _XStep = 0.005f;
    public float XStep
    {
        get => _XStep;
        set
        {
            _XStep = value;
            UpdateCircle();
        }
    }
    private float GenerateYPointForXValue(float x)
    {
        if (x > Math.Abs(Radius))
        {
            throw new ArgumentException("But the maximum x coord for a point in the circumference of a circle cannot be larger than it's radius");
        }
        return (float)Math.Sqrt((Math.Pow(Radius, 2) - Math.Pow(x, 2)));
    }
    private List<Vector2> GeneratePoints()
    {
        List<Vector2> PointsForRequiredCircle = new();
        //We start by constructing the upper semicircle
        for (float x = -Radius; x <= Radius; x += XStep)
        {
            PointsForRequiredCircle.Add(new Vector2(x, GenerateYPointForXValue(x)));
        }
        //Now we just construct the lower part from the opposite side
        for (float x = Radius; x >= -Radius; x -= XStep)
        {
            PointsForRequiredCircle.Add(new Vector2(x, -GenerateYPointForXValue(x)));
        }
        return PointsForRequiredCircle;
    }
    void UpdateCircle()
    {
        Points = GeneratePoints().ToArray();
    }
    public override void _Ready()
    {
        UpdateCircle();
    }

    public override void _Process(double delta)
    {
    }
}
