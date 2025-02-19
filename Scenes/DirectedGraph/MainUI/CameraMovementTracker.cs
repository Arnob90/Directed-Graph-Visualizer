using System;
using Godot;
namespace MouseMovementStatusSpace;
public abstract record MouseStatus;
public record Rest : MouseStatus;
public record Moving(Vector2 CurrentMousePosition) : MouseStatus
{
};

