using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Text;

public partial class GameOverScreen : Control
{
    private Button QuitButton;

    private AnimationPlayer animationPlayer;

    AudioStreamPlayer gameOverSound;

    bool gameQuit = false;

    public int daysCompleted;

    public override void _Ready()
    {
        QuitButton = GetNode<Button>("QuitButton");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        gameOverSound = GetNode<AudioStreamPlayer>("GameOverSfx");

        animationPlayer.AnimationFinished += OnAnimationFinished;

        QuitButton.ButtonDown += OnQuitButtonPressed;
    }

    public override void _Process(double delta)
    {
        if (GameManager.IsGameOver() == true && GetTree().Paused == false)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GetTree().Paused = true;
        animationPlayer.Play("blur");
        gameOverSound.Play();
    }

    private void OnQuitButtonPressed()
    {
        if (GetTree().Paused == true)
        {
            GetTree().Paused = false;
            animationPlayer.PlayBackwards("blur");
            gameQuit = true;
        }
    }

    private void OnAnimationFinished(StringName animationName)
    {
        if (animationName == "blur" && gameQuit == true)
        {
            GameManager.OnMainMenuTransition();
        }
    }
}
