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

    PackedScene scene;
    Node currentScene;

    SceneState currentSceneState;

    string MAIN_MENU_SCENE = "res://MainMenu.tscn";
   // string GAME_SCENE = "res://Game.tscn";



    public override void _Ready()
    {
        instance = this;

        DisplayServer.WindowSetTitle("Click & collect");

        currentSceneState = SceneState.MAIN_MENU;
        if(currentSceneState == SceneState.MAIN_MENU)
        {
            scene = ResourceLoader.Load<PackedScene>("res://MainMenu.tscn");
        }
        Node menuScene = scene.Instantiate();
        if (scene != null)
        {
            instance.AddChild(menuScene);
        }
        else
        {
            GD.Print("err scene empty");
        }
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

    static void OnMainMenuTransition()
    {
        

    }


}
