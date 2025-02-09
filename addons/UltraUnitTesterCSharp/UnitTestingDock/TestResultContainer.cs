using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UltraUnitTesterSpace.ResultTypesSpace;
using ResultLabelUISpace;
using UltraUnitTesterSpace.UiSpace;
namespace UltraUnitTesterSpace.UiSpace;
[Tool]
public partial class TestResultContainer : Tree
{
    TreeItem Root;
    List<MethodInfo> Functions;
    public override void _Ready()
    {
        Functions = new();
        Root = CreateItem();
        HideRoot = true;
        RefreshFunctionList();
    }
    private void ClearFunctionInfo()
    {
        Clear();
        Root = CreateItem();
    }
    public void RefreshFunctionList()
    {
        ClearFunctionInfo();
        foreach (var method in TestRunner.FindMethodsToTest())
        {
            Functions.Add(method);
            var child = Root.CreateChild();
            child.SetText(0, $"{method.Name}:{method.DeclaringType.FullName}");
        }
    }
    public void RunTests()
    {
        RefreshFunctionList();
        var testResults = TestRunner.RunTests(Functions);
        var nextNode = Root.GetFirstChild();
        int i = 0;
        while (nextNode != null)
        {
            var logText = nextNode.CreateChild();
            logText.SetText(0, testResults[i].Message);
            i = i + 1;
            nextNode = nextNode.GetNext();
        }
    }
}
