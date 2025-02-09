using Godot;
using System;
using System.Reflection;
using UltraUnitTesterSpace.ResultTypesSpace;
namespace UltraUnitTesterSpace.UiSpace;
public partial class FunctionInfoContainer : PanelContainer
{
	Label LogLabel;
	Label FunctionNameLabel;
	public override void _Ready()
	{
		LogLabel=GetNode<Label>("%ResultLabel");
	}
	public void LogResult(ResultType result)
	{
		LogLabel.Text=result.Message;
	}
	public void SetFunction(MethodInfo functionType)
	{
		FunctionNameLabel.Text=functionType.Name;
	}
}
