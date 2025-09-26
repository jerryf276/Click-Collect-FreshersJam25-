using Godot;
using System;

public partial class GameManager : Node2D
{
    static GameManager instance;

    enum SceneState
    {
        MAIN_MENU,
        IN_GAME
    }

    enum Players
    {
        Solo,
        Duo
    }

    bool isPaused;

    Node currentScene;

    string MAIN_MENU_SCENE = "res://MainMenu.tscn";
   // string GAME_SCENE = "res://Game.tscn";



    public override void _Ready()
    {
        instance = this;

        DisplayServer.WindowSetTitle("Click & collect");

        
    }


    public override void _Process(double delta)
    {


    }

    static public void OnSoloStart()
    {
        
    }


    static public void OnDuoStart()
    {

    }

    static void OnMenuTransition()
    {

    }


}
