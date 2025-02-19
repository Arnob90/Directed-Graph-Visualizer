using Godot;
using System;
using MouseMovementStatusSpace;
public partial class SplitterHandle : Label
{
    bool MouseOverHandle = false;
    MouseStatus PrevMouseStatus = new Rest();
    //Should probably be the topmost parent container
    [Export]
    Control ToScaleVertically;
    public override void _Ready()
    {
        MouseEntered += () => { MouseOverHandle = true; };
        MouseExited += () => { MouseOverHandle = false; };
    }

    public override void _Process(double delta)
    {
        if (MouseOverHandle && Input.IsActionJustPressed("Hold"))
        {
            PrevMouseStatus = new Moving(GetGlobalMousePosition());
        }
        if (MouseOverHandle && Input.IsActionPressed("Hold"))
        {
            //It should be impossible not to downcast here, so we make sure to do so directly instead of using is/as so as to not fail silently
            var PrevMouseStatusMotion = (Moving)PrevMouseStatus;
            var offset = GetGlobalMousePosition() - PrevMouseStatusMotion.CurrentMousePosition;
            var prevSize = ToScaleVertically.Size;
            GD.Print($"Offset: {offset}");
            GD.Print($"Prev size:{ToScaleVertically.Size}");
            //Up is negative in godot, but dragging up should increase size, not the other way around
            var updatedSize = prevSize with { Y = prevSize.Y - offset.Y };
            ToScaleVertically.Size = updatedSize;
            ToScaleVertically.Position = ToScaleVertically.Position with { Y = ToScaleVertically.Position.Y + offset.Y };
            PrevMouseStatusMotion = new Moving(GetGlobalMousePosition());
            GD.Print($"Updated size:{ToScaleVertically.Size}");

        }
        if (Input.IsActionJustReleased("Hold"))
        {
            PrevMouseStatus = new Rest();
        }
    }
}
