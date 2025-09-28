using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PauseMenu : Control
{
    private Button ResumeButton;
    private Button ControlsButton;
    private Button QuitButton;

    private AnimationPlayer animationPlayer;


  //  private static readonly PackedScene titleScreen = GD.Load<PackedScene>("res://Scenes/TitleScreen.tscn");
  //  private static readonly PackedScene controlsScreen = GD.Load<PackedScene>("res://Scenes/ControlsScreen.tscn");
    //private PackedScene currentScene;

    AudioStreamPlayer buttonPressed;
    AudioStreamPlayer Paused;

    bool gameQuit = false;

    public override void _Ready()
    {
        ResumeButton = GetNode<Button>("PanelContainer/VBoxContainer/ResumeButton");
      //  ControlsButton = GetNode<Button>("PanelContainer/VBoxContainer/ControlsButton");
        QuitButton = GetNode<Button>("PanelContainer/VBoxContainer/QuitButton");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");


        QuitButton.ButtonDown += OnQuitPressed;
        ResumeButton.ButtonDown += OnResumePressed;
        // ControlsButton.ButtonDown += OnControlsPressed;

        buttonPressed = GetNode<AudioStreamPlayer>("ButtonPressed");
        Paused = GetNode<AudioStreamPlayer>("Pause");
        animationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override void _Process(double delta)
    {
        
        if (Input.IsActionJustPressed("esc") && GetTree().Paused == false && (GetTree().CurrentScene.GetChild(1).Name == "CormacShopGen" || GetTree().CurrentScene.GetChild(1).Name == "SplitScreenScene"))
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
        buttonPressed.Play();
    }

    private void Pause()
    {
        GetTree().Paused = true;
        animationPlayer.Play("blur");
        Paused.Play();
    }


    private void OnAnimationFinished(StringName animationName)
    {

        if (animationName == "blur" && gameQuit == true)
        {
            GameManager.OnMainMenuTransition();
        }
    }


    private void OnQuitPressed()
    {
        //GetTree().ChangeSceneToPacked(titleScreen);
        if (GetTree().Paused == true)
        {
            GetTree().Paused = false;
            animationPlayer.PlayBackwards("blur");
            gameQuit = true;
            //GameManager.OnMainMenuTransition();
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
