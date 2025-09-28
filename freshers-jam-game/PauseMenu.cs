using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PauseMenu : Control
{
    private Button ResumeButton;
    private Button ControlsButton;
    private Button QuitButton;

    private AnimationPlayer animationPlayer;


    private static readonly PackedScene titleScreen = GD.Load<PackedScene>("res://Scenes/TitleScreen.tscn");
    private static readonly PackedScene controlsScreen = GD.Load<PackedScene>("res://Scenes/ControlsScreen.tscn");
    //private PackedScene currentScene;

    public override void _Ready()
    {
        ResumeButton = GetNode<Button>("PanelContainer/VBoxContainer/ResumeButton");
      //  ControlsButton = GetNode<Button>("PanelContainer/VBoxContainer/ControlsButton");
        QuitButton = GetNode<Button>("PanelContainer/VBoxContainer/QuitButton");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");


        QuitButton.ButtonDown += OnQuitPressed;
        ResumeButton.ButtonDown += OnResumePressed;
       // ControlsButton.ButtonDown += OnControlsPressed;


    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("esc") && GetTree().Paused == false && (GetTree().CurrentScene.Name == "CormacShopGen" || GetTree().CurrentScene.Name == "SplitScreenScene"))
        {
            Pause();
        }

        else if (Input.IsActionJustPressed("esc") && GetTree().Paused == true)
        {
            Resume();
        }
    }
    private void Resume()
    {
        GetTree().Paused = false;
        // AnimationPlayer.
        animationPlayer.PlayBackwards("blur");
    }

    private void Pause()
    {
        GetTree().Paused = true;
        animationPlayer.Play("blur");
    }


    private void OnQuitPressed()
    {
        //GetTree().ChangeSceneToPacked(titleScreen);
        if (GetTree().Paused == true)
        {
            GetTree().Paused = false;
            GameManager.OnMainMenuTransition();
        }
    }


    private void OnResumePressed()
    {
        if (GetTree().Paused == true)
        {
            Resume();
        }
    }

    //private void OnControlsPressed()
    //{
    //    GetTree().ChangeSceneToPacked(controlsScreen);
    //}


}
