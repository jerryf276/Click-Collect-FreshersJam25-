using Godot;
using System;

public partial class MainMenu : Control
{
    private Button singlePlayerButton;
    private Button multiPlayerButton;
    private Button controlsButton;
    private Button quitButton;

    private static readonly PackedScene Level = GD.Load<PackedScene>("res://Scenes/CormacShopGen2.tscn");


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
    }


    private void OnSinglePlayerPressed()
    {
        GetTree().ChangeSceneToPacked(Level);
    }


    private void OnMultiPlayerPressed()
    {

    }

    private void OnControlsPressed()
    {

    }


    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
