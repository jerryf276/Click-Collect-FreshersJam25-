using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

public partial class GameOverScreen : Control
{
    private Button QuitButton;

    private AnimationPlayer animationPlayer;

    AudioStreamPlayer gameOverSound;

    bool gameQuit = false;

    bool gameOverShown = false;

    public int daysCompleted;

    private Label productsDeliveredText;
    private Label listsCompletedText;
    private Label daysCompletedText;


    public override void _Ready()
    {
        QuitButton = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/QuitButton");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        gameOverSound = GetNode<AudioStreamPlayer>("GameOverSfx");
        daysCompletedText = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/DaysCompleted");
        listsCompletedText = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/ListsCompleted");
        productsDeliveredText = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/ProductsDelivered");
        animationPlayer.AnimationFinished += OnAnimationFinished;

        QuitButton.ButtonDown += OnQuitButtonPressed;
    }

    public override void _Process(double delta)
    {
        if (GameManager.IsGameOver() == true && GetTree().Paused == false && gameOverShown == false)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        productsDeliveredText.Text = "Products Delivered: " + GameManager.GetItemsDelivered();
        listsCompletedText.Text = "Lists Completed: " + GameManager.GetListsCompleted();
        daysCompletedText.Text = "Days Completed: " + GameManager.GetDaysCompleted(); 
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

        gameOverShown = true;
    }
}
