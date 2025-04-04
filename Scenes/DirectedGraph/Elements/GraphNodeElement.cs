using Godot;
using System;
[Tool]
public partial class GraphNodeElement : PanelContainer
{
	String _ElemStrRepr="";
	[Export]
	String ElemStrRepr
	{
		get=>_ElemStrRepr;
        set
        {
            _ElemStrRepr=value;
            if(IsNodeReady())
            {
				MainLabel.Text=value;
            }
        }
	}
	Label MainLabel;
	public override void _Ready()
	{
		MainLabel=GetNode<Label>("%MainLabel");
		MainLabel.Text=ElemStrRepr;
	}
}
