using Godot;
using System;
namespace MouseDetectorSpace;
public partial class MouseDetector : Area2D
{
	[Export]
	public Node2D ParentToDetect{get;private set;}
}
