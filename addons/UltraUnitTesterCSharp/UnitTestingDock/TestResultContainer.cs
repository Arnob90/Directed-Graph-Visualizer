using Godot;
using System;
using UltraUnitTesterSpace.ResultTypesSpace;
using ResultLabelUISpace;
using UltraUnitTesterSpace.UiSpace;
namespace UltraUnitTesterSpace.UiSpace;
[Tool]
public partial class TestResultContainer : VBoxContainer
{
	PackedScene ResultContainerScene;
	public override void _Ready()
	{
		RefreshFunctionList();
	}
	public void RefreshFunctionList()
	{
		var functionList=TestRunner.FindMethodsToTest();
		foreach(var function in functionList)
		{
			var resultContainer=ResultContainerScene.Instantiate<FunctionInfoContainer>();
			AddChild(resultContainer);
			resultContainer.SetFunction(function);
		}
	}
}
