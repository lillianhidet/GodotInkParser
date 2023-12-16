using Godot;
using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class storyManager : Node2D
{

	[Export(PropertyHint.GlobalFile, "*.json,")] 
	public string json {get; set;}

	[Export]
	Node storyNode;

	[Export]
	PackedScene textScene;

	[Export]
	PackedScene choiceScene;

	Story story;


	// For testing purposes only, replace this with an external hook
	public override void _Ready(){
		var inkJson = File.ReadAllText("stories/testStory.json");
		story = new Story(inkJson);


		continueStory();

	}

	

	public void continueStory(){

		if(story.canContinue){
			createText(story.Continue());

			if(story.currentChoices != null){
				handleChoices(story.currentChoices);
			}


		}

	}

	private void createText(String text){
		
		RichTextLabel instance = (RichTextLabel) textScene.Instantiate();
		instance.Text = text;
		storyNode.AddChild(instance);

	}

	private void handleChoices(List<Choice> choices){

		foreach(Choice c in choices){

			createChoice(c.text, c.index);

		}


	}

	private void createChoice(String text, int index){

		RichTextLabel instance = (RichTextLabel) choiceScene.Instantiate();
		instance.Text = text;
		Button bt = (Button) instance.GetChild(0); //Replace this with an explicit search for a button type child
		bt.Pressed += () => recieveChoiceInput(index);

		storyNode.AddChild(instance);



	}

	public void recieveChoiceInput(int index){

		story.ChooseChoiceIndex(index);
		continueStory();


	}
}
