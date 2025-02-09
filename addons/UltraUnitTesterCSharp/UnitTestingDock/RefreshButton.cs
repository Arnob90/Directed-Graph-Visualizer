using Godot;
using System;
using UltraUnitTesterSpace;
using UltraUnitTesterSpace.UiSpace;
[Tool]
public partial class RefreshButton : Button
{
    [Export]
    TestResultContainer TestResultsToRefresh;
    public override void _Ready()
    {
        Pressed += OnPress;
    }
    public void OnPress()
    {
        GD.Print("Refreshing...");
        TestResultsToRefresh.RefreshFunctionList();
    }

}
