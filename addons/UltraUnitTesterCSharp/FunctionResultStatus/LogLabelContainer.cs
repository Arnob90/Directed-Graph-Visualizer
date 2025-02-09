using Godot;
using System;
using UltraUnitTesterSpace.ResultTypesSpace;
namespace UltraUnitTesterSpace.UiSpace;
[Tool]
public partial class LogLabelContainer : Tree
{
    //We ensure this is private to prevent, say, .QueueFree() from outside
    private TreeItem Root;
    private TreeItem TreeLabel;
    public override void _Ready()
    {
        var Root = CreateItem();
        HideRoot = false;
    }

    public override void _Process(double delta)
    {
    }
    public void SetFunctionName(String functionName)
    {
        Root.SetText(0, functionName);
    }
    public void SetLog(ResultType resultToLog)
    {
        TreeLabel = Root.CreateChild();
        TreeLabel.SetText(0, resultToLog.Message);
    }
}
