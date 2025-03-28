using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CircleSpace;
using MouseMovementStatusSpace;
using MouseDetectorSpace;
namespace DiagramNodeSpace;
[Tool]
public partial class DiagramNode : Node2D
{
    [Export]
    public Label TextLabel;
    [Export]
    Container MainContainer;
    [Export]
    public Marker2D UpAnchor;
    [Export]
    public Marker2D DownAnchor;
    [Export]
    public Marker2D LeftAnchor;
    [Export]
    public Marker2D RightAnchor;
    [Export]
    public Marker2D CenterAnchor;
    [Export]
    private CircularLineTest CircularLine;
    public Vector2 TotalSize { get => MainContainer.Size; }
    public bool _MappedToSelf = false;
    [Export]
    public bool MappedToSelf { get => _MappedToSelf; set { _MappedToSelf = value; CallDeferred(MethodName.RefreshProperties); } }
    [Export]
    Line2D Border;
    private String _StrRepr = "";
    public String StrRepr
    {
        get => _StrRepr;
        set
        {
            _StrRepr = value;
            CallDeferred(MethodName.RefreshProperties);
        }
    }
    bool _Selected = false;
    [Export]
    public bool Selected
    {
        get => _Selected;
        set
        {
            _Selected = value;
            CallDeferred(MethodName.RefreshProperties);
        }
    }
    public bool IsHovering{get;private set;} = false;
    [Signal]
    public delegate void MouseClickedEventHandler(DiagramNode clickedNode);
	[Signal]
	public delegate void MovingEventHandler();
    public HashSet<DiagramNode> Neighbours;
	public MouseStatus CurrentMouseStatus{get;private set;}
	[Export]
	Area2D MouseDetector;
	//We use an immutable list to prevent modification from outside, which allowing internal modification in a simple way
	public ImmutableHashSet<DiagramNode> OverlappingNodes{get;private set;}
	public override void _Ready()
    {
		OverlappingNodes=ImmutableHashSet.Create<DiagramNode>();
        RefreshProperties();
        MouseDetector.MouseEntered += () => { IsHovering = true; GD.Print("Hovering"); };
        MouseDetector.MouseExited += () => { IsHovering = false; GD.Print("Not Hovering"); };
		MouseDetector.AreaEntered+=OverlappingNodeEntered;
		MouseDetector.AreaExited+=OverlappingNodeExited;
		CallDeferred(MethodName.SetBorder);
		MainContainer.Resized+=SetBorder;
		CurrentMouseStatus=new Rest();
    }
	private void SetBorder()
	{
		var mouseDetectorPoints=(MouseDetector.GetChildren()[0] as CollisionPolygon2D).Polygon;
		Border.Points=mouseDetectorPoints;
	}
	public override void _Input(InputEvent @event)
	{
        if (!Engine.IsEditorHint())
        {
            if (Input.IsActionJustPressed("LMB") && IsHovering)
            {
                EmitSignal(SignalName.MouseClicked,this);
                GD.Print("Clicked");
                // Selected = true;
            }
            if (Input.IsActionJustPressed("LMB") && !IsHovering)
            {
                GD.Print("Unclicked");
                Selected = false;
            }
        }
	}
	public override void _Process(double delta)
	{
		if(Selected)
		{
			if(Input.IsActionJustPressed("LMB"))
			{
				CurrentMouseStatus=new Moving(GetLocalMousePosition());
				EmitSignal(SignalName.Moving);
			}
			else if(Input.IsActionPressed("LMB") && CurrentMouseStatus is Moving mouseStatusCasted)
			{
				var dragOffset=GetLocalMousePosition()-mouseStatusCasted.CurrentMousePosition;
				Position=Position+dragOffset;
				mouseStatusCasted=new Moving(GetLocalMousePosition());
				EmitSignal(SignalName.Moving);
			}
			else if(Input.IsActionJustReleased("LMB"))
			{
				CurrentMouseStatus=new Rest();
			}
		}
	}
    public void RefreshProperties()
    {
        CircularLine.Visible = MappedToSelf;
        TextLabel.Text = StrRepr;
        Border.Visible = Selected;
    }
	private void OverlappingNodeEntered(Area2D MaybeAreaOfOverlappingNode)
	{
		if(MaybeAreaOfOverlappingNode is MouseDetector mouseDetectorOfNode)
		{
			var detectedNode=mouseDetectorOfNode.ParentToDetect;
			if(detectedNode is DiagramNode overlappingDiagramNode)
			{
				OverlappingNodes=OverlappingNodes.Add(overlappingDiagramNode);
			}
		}
		
	}
	private void OverlappingNodeExited(Area2D MaybeAreaOfOverlappingNode)
	{
		if(MaybeAreaOfOverlappingNode is MouseDetector mouseDetectorOfNode)
		{
			var detectedNode=mouseDetectorOfNode.ParentToDetect;
			if(detectedNode is DiagramNode overlappingDiagramNode)
			{
				OverlappingNodes=OverlappingNodes.Remove(overlappingDiagramNode);
			}
		}
	}
}
