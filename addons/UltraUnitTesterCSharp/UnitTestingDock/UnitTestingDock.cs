using Godot;
using System;
using UltraUnitTesterSpace;
using UltraUnitTesterSpace.UiSpace;
public partial class UnitTestingDock : PanelContainer
{
    [Export]
    PackedScene FunctionResultContainerScene;
    public override void _Ready()
    {
        RefreshFunctionList();
    }

    public override void _Process(double delta)
    {
    }

    public void RefreshFunctionList()
    {
        foreach (var method in TestRunner.FindMethodsToTest())
        {
            var methodInfoContainer = FunctionResultContainerScene.Instantiate<FunctionInfoContainer>();
            AddChild(methodInfoContainer);
            methodInfoContainer.SetFunction(method);
        }
    }
}
