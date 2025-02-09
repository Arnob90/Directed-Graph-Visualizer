using Godot;
using System;
using UltraUnitTesterSpace;
using UltraUnitTesterSpace.UiSpace;
[Tool]
public partial class RunTestsButton : Button
{
    [Export]
    TestResultContainer ToAddTestsTo;
    public override void _Ready()
    {
        GD.Print("I am ready");
        Pressed += OnPress;
    }

    public override void _Process(double delta)
    {
    }
    public void OnPress()
    {
        ToAddTestsTo.RunTests();
    }
}
