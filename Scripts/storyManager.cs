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

	private bool awaitingDecision = false;


	//Create a stack for tweening object opacity, only releasing one object per set interval?

	// For testing purposes only, replace this with an external hook
	public override void _Ready(){
		var inkJson = File.ReadAllText("stories/testStory.json");
		story = new Story(inkJson);


		continueStory();

	}


	public void recieveContine(){
		if(!awaitingDecision){
			continueStory();
		}
	}
	

	public void continueStory(){

		if(story.canContinue){

			createText(story.Continue());

		}else if(story.currentChoices != null){

			handleChoices(story.currentChoices);

	
		}
	}

	private void createText(String text){
		
		RichTextLabel instance = (RichTextLabel) textScene.Instantiate();
		instance.ParseBbcode(text);
		
		storyNode.AddChild(instance);

		//Fade in
		fadeIn(instance);

		

	}



	private void handleChoices(List<Choice> choices){

		awaitingDecision = true;

		currentChoices = new List<RichTextLabel>();

		foreach(Choice c in choices){

			createChoice(c.text, c.index);

		}


	}

	private void createChoice(String text, int index){

		RichTextLabel instance = (RichTextLabel) choiceScene.Instantiate();
		instance.Text = text;
		Button bt = (Button) instance.GetChild(0); //Replace this with an explicit search for a button type child?
		bt.Pressed += () => recieveChoiceInput(index);

		fadeIn(instance);

		currentChoices.Add((RichTextLabel)bt.GetParent());
		storyNode.AddChild(instance);



	}

	private void recieveChoiceInput(int index){

		awaitingDecision = false;

		createText("[i]" + currentChoices[index].Text + "[/i]");

		foreach(Node n in currentChoices){
			n.QueueFree();
		}

		currentChoices.Clear(); //Not necessary but as a failsafe

		story.ChooseChoiceIndex(index);
		
		


	}

	void fadeIn(RichTextLabel n){

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(n, "modulate", new Color(1,1,1,1), 5.0);


	}



}
