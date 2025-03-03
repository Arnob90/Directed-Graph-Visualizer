using Godot;
using System;
[GlobalClass, Tool]
public partial class BidirectionalLineInfo : AbstractDirectionalLineInfo
{
    [Export(PropertyHint.Range, "0,1,")]
    public float DistanceFromSecondArrowRatio = 0.2f;
}
