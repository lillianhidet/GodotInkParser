using Godot;
using Ink.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

//I may want to create a class which interfaces with the game engine, seperate from this one,
//To allow the underlying logic to be engine agnostic?

public partial class storyManager : Node2D
{

	[Export(PropertyHint.GlobalFile, "*.json,")] 
	public string storyJson {get; set;}

	[Export(PropertyHint.GlobalFile, "*.json,")] 
	public string varsJson {get; set;}

	[Export]
	Node storyNode;

	[Export]
	PackedScene textScene;

	[Export]
	PackedScene choiceScene;

	private Story story;
	private Story vars;

	private List<RichTextLabel> currentChoices;

	private bool awaitingDecision = false;


	//Create a stack for tweening object opacity, only releasing one object per set interval?

	// For testing purposes only, replace this with an external hook
	public override void _Ready(){

		storyJson = storyJson.TrimPrefix("res://");
		varsJson = varsJson.TrimPrefix("res://");

		var storyText = File.ReadAllText(storyJson);
		var varsText = File.ReadAllText(varsJson);

		story = new Story(storyText);
		vars = new Story(varsText);

		continueStory();

	}


	//Called via external input from player (i.e clicking)
	public void recieveContine(){
		if(!awaitingDecision){
			continueStory();
		}
	}
	
	//Main driver of the story
	public void continueStory(){

		if(story.canContinue){

			createText(story.Continue());

		}else if(story.currentChoices != null){

			handleChoices(story.currentChoices);

	
		}
	}

	//Creates a text label 
	private void createText(String text){
		
		RichTextLabel instance = (RichTextLabel) textScene.Instantiate();
		instance.ParseBbcode(text);

		storyNode.AddChild(instance);

		//Fade in
		fadeIn(instance);

		

	}


	//Recieves a list of current choices
	private void handleChoices(List<Choice> choices){

		awaitingDecision = true;

		currentChoices = new List<RichTextLabel>();

		foreach(Choice c in choices){

			createChoice(c.text, c.index);

		}


	}

	//Creates a choice button, and attaches a listener to it.
	private void createChoice(String text, int index){

		RichTextLabel instance = (RichTextLabel) choiceScene.Instantiate();
		instance.Text = text;
		Button bt = (Button) instance.GetChild(0); //Replace this with an explicit search for a button type child?
		bt.Pressed += () => recieveChoiceInput(index);

		fadeIn(instance);

		currentChoices.Add((RichTextLabel)bt.GetParent());
		storyNode.AddChild(instance);



	}

	//A listener attached to each choice button created, returns the index of the selected choice.
	private void recieveChoiceInput(int index){

		awaitingDecision = false;

		createText("[i]" + currentChoices[index].Text + "[/i]");

		foreach(Node n in currentChoices){
			n.QueueFree();
		}

		currentChoices.Clear(); //Not necessary but as a failsafe

		story.ChooseChoiceIndex(index);

	}

	private void handleTags(){}


	/*Generic typing seems the best way to achieve this without creating
	a seperate function for each variable type, or returning just strings (would cause bloat elsewhere)
	The tag parser will know the type, so type consistency should be maintained*/

	private T getVar<T>(string name){

		var variable = vars.variablesState[name];

		return (T)Convert.ChangeType(variable, typeof(T));
	}

	private void setVar<T>(string var, T value){

		vars.variablesState[var] = value;

	}


//Could it make sense to create a utils class for functions like this?
	private void fadeIn(RichTextLabel n){

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(n, "modulate", new Color(1,1,1,1), 5.0);

	}

	



}
