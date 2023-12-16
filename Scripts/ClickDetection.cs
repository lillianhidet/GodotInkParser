using Godot;
using System;
using System.Diagnostics;

public partial class ClickDetection : ColorRect{

//Would really like this to be cleaned up. Better to get at runtime?
	[Export]
	storyManager story;

    public override void _Ready(){
		if(story == null){
		GD.PushWarning("No storymanager class in clickDetection");
		}
    }

    public override void _GuiInput(InputEvent @event){

        if(@event is InputEventMouseButton mb){

			if(mb.ButtonIndex == MouseButton.Left && mb.Pressed){

				story.recieveContine();

			}


		}


    }
}
