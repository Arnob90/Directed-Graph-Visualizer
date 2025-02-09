using UltraUnitTesterSpace;
using Godot;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltraUnitTesterSpace.ResultTypesSpace;
namespace UltraUnitTesterSpace;
public class TestRunner
{
    public static List<MethodInfo> FindMethodsToTest()
    {
        var executingAssembly = typeof(TestRunner).Assembly;
        var classesInAssembly = executingAssembly.GetTypes();
        var classesToTest = classesInAssembly.Where((type) => type.GetCustomAttribute<TestClassAttribute>(false) != null);
        var methodsToTest = new List<MethodInfo>();
        foreach (var type in classesToTest)
        {
            var methods = type.GetMethods().Where((method) => method.IsStatic);
            foreach (var method in methods)
            {
                if (method.GetCustomAttribute<TestMethodAttribute>(false) != null && method.ReturnType.IsAssignableFrom(typeof(ResultType)))
                {
                    GD.Print(method.Name);
                    methodsToTest.Add(method);
                }
            }
        }
        return methodsToTest;
    }
    public static List<ResultType> RunTests(IEnumerable<MethodInfo> methodsToTest)
    {
        GD.Print("I am here");
        foreach (var method in methodsToTest)
        {
            GD.Print("Printing method name: ");
            GD.Print(nameof(method));
        }
        if (!methodsToTest.All((method) => method.ReturnType.IsAssignableFrom(typeof(ResultType)) && method.IsStatic))
        {
            throw new ArgumentException("The return type of the given methods must be of type ResultType. Also, it must be static");
        }
        var results = new List<ResultType>();
        foreach (var method in methodsToTest)
        {
            try
            {
                var result = (ResultType)method.Invoke(null, null);
                results.Add(result);
            }
            catch (Exception err)
            {
                results.Add(new ExceptionType(err.ToString()));
            }
        }
        return results;
    }
}
