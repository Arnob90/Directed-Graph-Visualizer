using System;
using Godot;
namespace ArrowDirectionalLineSpace;
[Tool]
public partial class ArrowDirectionalLine : Line2D
{
    [Export]
    public float Length
    {
        get => _Length;
        set
        {
            _Length = value;
            CallDeferred(MethodName.DrawArrow);
        }
    }
    [Export(PropertyHint.Range, "-360,360,")]
    public float AngleDegrees
    {
        get => _AngleDegrees;
        set
        {
            _AngleDegrees = value;
            CallDeferred(MethodName.DrawArrow);
        }
    }
    private float _Length;
    private float _AngleDegrees;
    public override void _Ready()
    {
        DrawArrow();
    }
    private void DrawArrow()
    {
        Vector2 horizontalVec = Vector2.Right;
        var requiredRotation = Mathf.Pi - Mathf.DegToRad(AngleDegrees);
        var upperArrowEndpoint = horizontalVec.Rotated(requiredRotation) * Length;
        var lowerArrowEndpoint = horizontalVec.Rotated(-requiredRotation) * Length;
        Points = new Vector2[] { upperArrowEndpoint, Vector2.Zero, lowerArrowEndpoint };
    }
}
