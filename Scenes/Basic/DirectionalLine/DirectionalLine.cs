using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ArrowDirectionalLineSpace;
using DirectionalLineSpace;
namespace DirectionalLineSpace;
[Tool]
public partial class DirectionalLine : Line2D
{
    Vector2[] previousPointsList = new Vector2[] { };
    double accumulatedTime = 0;
    [Export]
    double RerenderEvery = 0.1;
    //Will be Dupelicate()'ed if necessary
    [Export]
    PackedScene ArrowScene;
    private AbstractDirectionalLineInfo _Info;
    [Export]
    public AbstractDirectionalLineInfo Info
    {
        get => _Info;
        set
        {
            _Info = value;
            RequestRerender = true;
        }
    }
    bool RequestRerender = false;
    public override void _Ready()
    {
		if(Info==null)
		{
			Info=new SingleDirectionalLineInfo();
		}
    }
    public override void _Process(double delta)
    {
        if (accumulatedTime >= RerenderEvery)
        {
            CleanupArrows();
            if (Info == null || Points.Length != 2)
            {
            }
            else if (Points.Length == 2)
            {
                previousPointsList = Points.ToArray();
                RequestRerender = false;
                if (Info is SingleDirectionalLineInfo)
                {
                    DrawArrow((Points[0], Points[1]), Info.ToPlaceArrowOnLerp);
                }
                if (Info is BidirectionalLineInfo bidirectionalInfo)
                {
                    var toPlaceOn = bidirectionalInfo.ToPlaceArrowOnLerp;
                    var toPlaceOnSecond = Mathf.Clamp(toPlaceOn + bidirectionalInfo.DistanceFromSecondArrowRatio, 0, 1);
                    var toPlaceOnFirst = Mathf.Clamp(toPlaceOn - bidirectionalInfo.DistanceFromSecondArrowRatio, 0, 1);
                    DrawArrow((Points[0], Points[1]), toPlaceOnFirst, true);
                    DrawArrow((Points[0], Points[1]), toPlaceOnSecond, false);
                }
            }
            accumulatedTime = 0;
        }
        accumulatedTime += delta;
    }
    private void DrawArrow((Vector2, Vector2) pointPair, float toPlaceArrowOnLerp = 0.5f, bool mirrored = false)
    {
        var (firstPoint, secondPoint) = pointPair;
        var toPlaceArrowOn = GetPointInLine(firstPoint, secondPoint)(toPlaceArrowOnLerp);
        var arrow = ArrowScene.Instantiate<ArrowDirectionalLineSpace.ArrowDirectionalLine>();
        var rotationAngle = firstPoint.DirectionTo(secondPoint).Angle();
        arrow.Rotation = rotationAngle;
        arrow.Length = 20;
        arrow.GlobalPosition = toPlaceArrowOn;
        arrow.Width = Width;
        if (mirrored)
        {
            arrow.Scale = arrow.Scale with { X = -arrow.Scale.X };
        }
        AddChild(arrow);
    }
    private void CleanupArrows()
    {
        foreach (var maybeArrow in GetChildren())
        {
            if (maybeArrow is ArrowDirectionalLine)
            {
                maybeArrow.QueueFree();
            }
        }
    }
    // private void DrawTriangle((Vector2, Vector2) pointPair)
    // {
    //     var (firstPoint, secondPoint) = pointPair;
    //     var angleBetweenPoints = firstPoint.DirectionTo(secondPoint);
    //     var perpendicular = (secondPoint - firstPoint).Normalized().Rotated(90);
    //     var oppositePerpendicular = (secondPoint - firstPoint).Normalized().Rotated(-90);
    //     var lineFunction = GetPointInLine(pointPair.Item1, pointPair.Item2);
    //     var midpoint = lineFunction(0.5F);
    //     var startPointOfTriangle = lineFunction(0.4F);
    //     var upperPointOfTriangle = startPointOfTriangle + perpendicular * 50;
    //     var lowerPointOfTriangle = startPointOfTriangle + oppositePerpendicular * 50;
    //     var requiredTriangle = (Polygon2D)ToDrawPolygonWith.Duplicate();
    //     OriginalPolygonDupelicates.Add(requiredTriangle);
    //     requiredTriangle.Polygon = new Vector2[] { midpoint, upperPointOfTriangle, lowerPointOfTriangle, midpoint };
    //     AddChild(requiredTriangle);
    //
    // }
    private Func<float, Vector2> GetPointInLine(Vector2 firstPoint, Vector2 secondPoint)
    {
        return (float progress) =>
        {
            if (!(0 <= progress && progress <= 1))
            {
                throw new ArgumentException("Lerp progress is out of range and must be between 0 and 1 inclusive");
            }
            return firstPoint + progress * (secondPoint - firstPoint);
        };
    }
}
