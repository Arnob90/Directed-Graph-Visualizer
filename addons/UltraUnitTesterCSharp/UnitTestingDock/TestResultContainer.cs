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
    List<String> FunctionNames;
    public override void _Ready()
    {
        FunctionNames = new();
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
            FunctionNames.Add($"{method.Name}:{method.DeclaringType.AssemblyQualifiedName}");
            var child = Root.CreateChild();
            child.SetText(0, $"{method.Name}:{method.DeclaringType.FullName}");
        }
    }
	private static MethodInfo GetMethodFromName(String methodName)
	{
		var methodsToTest=TestRunner.FindMethodsToTest();
		var requiredMethods=methodsToTest.Where((method)=>$"{method.Name}:{method.DeclaringType.AssemblyQualifiedName}"==methodName);
		if(requiredMethods.Count()>1)
		{
			throw new InvalidOperationException($"Name collision between methods. Method name is ${methodName}");
		}
		if(requiredMethods.Count()==0)
		{
			throw new InvalidOperationException($"Required method ${methodName} is absent. Check if the code has changed with refreshing");
		}
		return requiredMethods.First();
	}
    public void RunTests()
    {
        RefreshFunctionList();
        var testResults = TestRunner.RunTests(FunctionNames.Select((functionName)=>GetMethodFromName(functionName)));
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
