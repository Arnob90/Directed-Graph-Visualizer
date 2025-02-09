using Godot;
using System;

public partial class NetworkFileOpener : Button
{
	[Signal]
	public delegate void FileOpenedEventHandler(String path);
	// Called when the node enters the scene tree for the first time.
	private FileDialog FileDialog;
	public override void _Ready()
	{
		FileDialog=GetNode<FileDialog>("%FileDialog");
		FileDialog.Transient=true;
		FileDialog.FileMode=FileDialog.FileModeEnum.OpenFile;
		FileDialog.UseNativeDialog=true;
		Pressed+=OpenFileDialog;
		FileDialog.FileSelected+=(String path)=>{EmitSignal(SignalName.FileOpened,ProjectSettings.GlobalizePath(path));};
		FileOpened+=(path)=>{GD.Print(path);};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void OpenFileDialog()
	{
		FileDialog.Show();
	}
}
