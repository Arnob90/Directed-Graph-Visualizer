using Godot;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
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
        var functionList = TestRunner.FindMethodsToTest();
        foreach (var function in functionList)
        {
            var resultContainer = ResultContainerScene.Instantiate<FunctionInfoContainer>();
            AddChild(resultContainer);
            resultContainer.SetFunction(function);
        }
    }

    private List<FunctionInfoContainer> GetAllTestContainers()
    {
        var requiredContainers = new List<FunctionInfoContainer>();
        foreach (var child in GetChildren())
        {
            if (child is FunctionInfoContainer requiredContainer)
            {
                requiredContainers.Add(requiredContainer);
            }
        }
        return requiredContainers;
    }

    public void RunTests()
    {
        RefreshFunctionList();
        var functionList = TestRunner.FindMethodsToTest();
        var results = TestRunner.RunTests(functionList);
        var containers = GetAllTestContainers();
        foreach (var container in containers)
        {
            var underlyingMethod = container.GetFunction();
            var indexInList = functionList.FindIndex((elem) => elem == underlyingMethod);
            var result = results[indexInList];
            container.LogResult(result);
        }
    }
}
