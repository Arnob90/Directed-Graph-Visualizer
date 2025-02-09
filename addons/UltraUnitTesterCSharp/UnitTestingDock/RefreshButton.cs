using Godot;
using System;
using UltraUnitTesterSpace;
using UltraUnitTesterSpace.UiSpace;
public partial class RefreshButton : TextureButton
{
    [Export]
    TestResultContainer TestResultsToRefresh;
    public override void _Ready()
    {
        Pressed += OnPress;
    }
    public void OnPress()
    {
        TestResultsToRefresh.RefreshFunctionList();
    }

}
