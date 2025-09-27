using Godot;
using System;
using System.IO;






public partial class GameManager : Node2D
{
    static GameManager instance;

    enum SceneState
    {
        MAIN_MENU,
        IN_GAME_SOLO,
        IN_GAME_DUO
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

    string MAIN_MENU_SCENE = "res://Scenes/TitleScreen.tscn";
  



    public override void _Ready()
    {
        instance = this;

        DisplayServer.WindowSetTitle("Click & collect");

        currentSceneState = SceneState.MAIN_MENU;
        if(currentSceneState == SceneState.MAIN_MENU)
        {
            scene = ResourceLoader.Load<PackedScene>(MAIN_MENU_SCENE);
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

    public static void OnSoloStart()
    {
        instance.currentSceneState = SceneState.IN_GAME_SOLO;

        //note code below could be made into seprate func for code reusability
        instance.GetTree().CurrentScene.QueueFree();
        instance.scene = ResourceLoader.Load<PackedScene>("res://Scenes/CormacShopGen.tscn");
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.AddChild(newScene);
        instance.GetTree().CurrentScene = newScene;
    }


    static public void OnDuoStart()
    {
        instance.currentSceneState = SceneState.IN_GAME_DUO;

        //note code below could be made into seprate func for code reusability
        instance.GetTree().CurrentScene.QueueFree();
        instance.scene = ResourceLoader.Load<PackedScene>("res://SplitScreenScene.tscn");
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.AddChild(newScene);
        instance.GetTree().CurrentScene = newScene;
    }

    static void OnMainMenuTransition()
    {
        

    }


}
