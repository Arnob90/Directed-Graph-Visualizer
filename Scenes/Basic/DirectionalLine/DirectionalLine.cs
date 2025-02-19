using Godot;
using System;
namespace DirectionalLineSpace;
public partial class DirectionalLine : Line2D
{
    Vector2[] previousPointsList = new Vector2[] { };
    double accumulatedTime = 0;
    [Export]
    double rerenderEvery = 0.1;
    public override void _Ready()
    {
    }
    public override void _Process(double delta)
    {
        if (accumulatedTime >= rerenderEvery && Points.Length >= 2 && previousPointsList != Points)
        {
            for (int i = 0; i < Points.Length - 1; ++i)
            {
                var firstPoint = Points[i];
                var secondPoint = Points[i + 1];
                DrawArrow(25, Mathf.DegToRad(43), (firstPoint, secondPoint));
            }
            accumulatedTime = 0;
            previousPointsList = Points;
            return;
        }
        accumulatedTime += delta;
    }
    private void DrawArrow(float length, float angle, (Vector2, Vector2) pointPair)
    {
        var lengthSqared = Math.Pow(length, 2);
        float offsetX = (float)(length / (Math.Sqrt(1 + Math.Pow(Math.Tan(angle), 2))));
        var offsetY = (float)(Math.Sqrt(lengthSqared - (lengthSqared / (1 + Math.Pow(Math.Tan(angle), 2)))));
        var offsetVec = new Vector2(offsetX, offsetY);
        var (firstPoint, secondPoint) = pointPair;
        var midpoint = (firstPoint + secondPoint) / 2;
        var requiredArrow = midpoint - offsetVec;
        var arrowLine = new Line2D();
        arrowLine.GlobalPosition = GlobalPosition;
        arrowLine.Points = new Vector2[] { midpoint, requiredArrow };
        AddChild(arrowLine);
        var secondArrowLine = (Line2D)arrowLine.Duplicate();
        var offsetVecMirrored = (offsetVec with { X = -offsetVec.X });
        var requiredArrowFlipped = midpoint + offsetVecMirrored;
        secondArrowLine.Points = new Vector2[] { midpoint, requiredArrowFlipped };
        AddChild(secondArrowLine);
        arrowLine.Width = Width;
        secondArrowLine.Width = Width;
    }
    private Line2D DrawPerpendicular(double length)
    {
        var firstPoint = Points[0];
        var secondPoint = Points[1];
        var secondLine = new Line2D();
        var rotationPivot = new Node2D();
        AddSibling(rotationPivot);
        secondLine.Points = Points;
        var midpoint = (secondPoint + firstPoint) / 2;
        rotationPivot.Position = midpoint;
        rotationPivot.AddChild(secondLine);
        secondLine.GlobalPosition = GlobalPosition;
        rotationPivot.RotationDegrees = 90;
        return secondLine;
    }
}
