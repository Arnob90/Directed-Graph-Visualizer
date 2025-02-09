#if TOOLS
using Godot;
using System;

[Tool]
public partial class UltraUnitTestCSharp : EditorPlugin
{
	private Control Dock;
	public override void _EnterTree()
	{
		Dock=GD.Load<PackedScene>("res://addons/UltraUnitTesterCSharp/UnitTestingDock/UnitTestingDock.tscn").Instantiate<Control>();
		AddControlToBottomPanel(Dock,"Unit Tests");
	}

	public override void _ExitTree()
	{
		RemoveControlFromBottomPanel(Dock);
		Dock.QueueFree();
	}
}
#endif
