using Godot;
using System;

public partial class MainMenu : Control
{
	private Button singlePlayerButton;
	private Button multiPlayerButton;
	private Button controlsButton;
	private Button quitButton;
    AudioStreamPlayer buttonPressed;

    //public Node GameManager;

    private static readonly PackedScene level = GD.Load<PackedScene>("res://Scenes/CormacShopGen2.tscn");
	private static readonly PackedScene controlsScreen = GD.Load<PackedScene>("res://Scenes/ControlsScreen.tscn");


	public override void _Ready()
	{
		singlePlayerButton = GetNode<Button>("TextureRect/MarginContainer/HBoxContainer/VBoxContainer/Single_Player_Button");
		multiPlayerButton = GetNode<Button>("TextureRect/MarginContainer/HBoxContainer/VBoxContainer/Multi_Player_Button");
		controlsButton = GetNode<Button>("TextureRect/MarginContainer/HBoxContainer/VBoxContainer/Controls_Button");
		quitButton = GetNode<Button>("TextureRect/MarginContainer/HBoxContainer/VBoxContainer/Quit_Button");

	   
		singlePlayerButton.ButtonDown += OnSinglePlayerPressed;
		multiPlayerButton.ButtonDown += OnMultiPlayerPressed;
		controlsButton.ButtonDown += OnControlsPressed;
		quitButton.ButtonDown += OnQuitButtonPressed;
        buttonPressed = GetNode<AudioStreamPlayer>("ButtonPressed");
    }


	private void OnSinglePlayerPressed()
	{
		buttonPressed.Play();
	   GameManager.OnSoloStart();
		
	}


	private void OnMultiPlayerPressed()
	{
        buttonPressed.Play();
        GameManager.OnDuoStart();
	}

	private void OnControlsPressed()
	{
        buttonPressed.Play();
        GetTree().ChangeSceneToPacked(controlsScreen);
	//	((ControlsScreen)GetTree().CurrentScene).SetPreviousScene(GD.Load<PackedScene>(""));

    }


	private void OnQuitButtonPressed()
	{
        buttonPressed.Play();
        GetTree().Quit();
	}
}
