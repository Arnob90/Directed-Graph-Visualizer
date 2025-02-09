using Godot;
using System;

public partial class TreeTestScene : Tree
{
    TreeItem Root;
    public override void _Ready()
    {
        Root = CreateItem();
        Root.SetText(0, "Hello There!");
        var child = Root.CreateChild();
        child.SetText(0, "Sus");
    }

    public override void _Process(double delta)
    {
    }
}
