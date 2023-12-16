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

	private Story story;

	private List<RichTextLabel> currentChoices;


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
		instance.ParseBbcode(text);
		storyNode.AddChild(instance);

	}

	private void handleChoices(List<Choice> choices){

		currentChoices = new List<RichTextLabel>();

		foreach(Choice c in choices){

			createChoice(c.text, c.index);

		}


	}

	private void createChoice(String text, int index){

		RichTextLabel instance = (RichTextLabel) choiceScene.Instantiate();
		instance.Text = text;
		Button bt = (Button) instance.GetChild(0); //Replace this with an explicit search for a button type child
		bt.Pressed += () => recieveChoiceInput(index);

		currentChoices.Add((RichTextLabel)bt.GetParent());
		storyNode.AddChild(instance);



	}

	public void recieveChoiceInput(int index){

		createText("[i]" + currentChoices[index].Text + "[/i]");

		foreach(Node n in currentChoices){
			n.QueueFree();
		}

		currentChoices.Clear(); //Not necessary but as a failsafe

		story.ChooseChoiceIndex(index);
		continueStory();


	}
}
