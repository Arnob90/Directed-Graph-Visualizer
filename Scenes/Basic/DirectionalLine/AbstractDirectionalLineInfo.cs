using Godot;
using System;
[GlobalClass, Tool]
public abstract partial class AbstractDirectionalLineInfo : Resource
{
    [Export(PropertyHint.Range, "0,1,")]
    public float ToPlaceArrowOnLerp = 0.5f;
}
