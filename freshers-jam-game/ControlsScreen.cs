using Godot;
using System;

public partial class ControlsScreen : Control
{
    private Button backButton;
    private static readonly PackedScene mainMenu = GD.Load<PackedScene>("res://Scenes/TitleScreen.tscn");
    private PackedScene previousScene;
    AudioStreamPlayer buttonPressed;


    public override void _Ready()
    {
        backButton = GetNode<Button>("BackButton");

        backButton.ButtonDown += OnBackButtonPressed;
        buttonPressed = GetNode<AudioStreamPlayer>("ButtonPressed");

    }


    private void OnBackButtonPressed()
    {

        //if (GetTree().CurrentScene.Name == "TitleScreen") { }
        buttonPressed.Play();
        GameManager.OnMainMenuTransition();
    }


    //public void GetPreviousScene()
    //{

   // }

    public void SetPreviousScene(PackedScene prevScene)
    {
        previousScene = prevScene;
    }


}
