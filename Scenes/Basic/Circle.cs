using Godot;
using System;
[Tool]
public partial class Circle : Node2D
{
	float _Radius=10;
	[Export]
	float Radius
	{
		get=>_Radius;
		set
		{
			_Radius=value;
			QueueRedraw();
		}
	}
	public override void _Draw()
	{
		DrawCircle(Position,Radius,Color.FromHtml("#FFFFFF"),true,-1,true);
	}
}
