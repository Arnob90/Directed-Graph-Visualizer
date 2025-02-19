using Godot;
using System;

public partial class CurveDraw : Node2D
{
    private Curve2D curve = new Curve2D();
    private int resolution = 100; // Number of segments for smoothness

    public override void _Ready()
    {
        // Add points to the curve
        curve.AddPoint(new Vector2(100, 300));
        curve.AddPoint(new Vector2(200, 100));
        curve.AddPoint(new Vector2(300, 300));
    }
    public override void _Draw()
    {
    }
}
