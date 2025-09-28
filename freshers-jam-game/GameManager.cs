using Godot;
using System;
using System.Collections.Generic;
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
    Timer dayTimer;
    ShopGenerator shopGenerator;
    Node menuScene;
    Control pauseMenu;


    public int quota;
    public int currentProgress;
    public int dayNum=1;
    public int itemsPerCat=2;
    public int MapSize = 10;

    public List<Player> Playerlist = new List<Player>();

    SceneState currentSceneState;

    string MAIN_MENU_SCENE = "res://Scenes/TitleScreen.tscn";




    public override void _Ready()
    {
        instance = this;

        DisplayServer.WindowSetTitle("Click & collect");

        dayTimer = GetNode<Timer>("DayTime");
        instance.dayTimer.WaitTime = 100;

        currentSceneState = SceneState.MAIN_MENU;

        if (currentSceneState == SceneState.MAIN_MENU)
        {
            scene = ResourceLoader.Load<PackedScene>(MAIN_MENU_SCENE);
        }

        menuScene = scene.Instantiate();

        if (scene != null)
        {
            instance.GetTree().Root.GetChild(0).AddChild(menuScene);
            
            instance.currentScene= menuScene;
        }
        else
        {
            GD.Print("err scene empty");
        }
        
    }


    public override void _Process(double delta)
    {

        //if (Input.IsActionJustPressed("esc") && GetTree().Paused == false && (GetTree().CurrentScene.Name == "CormacShopGen" || GetTree().CurrentScene.Name == "SplitScreenScene"))
        //{
        //    //instance.pauseMenu.Pause();
        //}

        

        if (currentProgress==quota)
        {
            if((dayNum & 3)==0)
            {
                quota++;
                itemsPerCat = 3;

            }
            else if((dayNum&5)==0)
            {
                MapSize++;
            }
            else
            {
                itemsPerCat++;
            }

            onNewday(quota,MapSize,MapSize,itemsPerCat);
        }
    }

    public static void AddtoPlayer(Player player)
    {
        instance.Playerlist.Add(player);
    }

    public static Player GetPlayers(int playernumber)
    {
        foreach (Player player in instance.Playerlist)
        {
            if (player.playerNumber == playernumber)
            {
                return player;
            }
        }
        return null;
       
    }

    public static void OnSoloStart()
    {

       

        instance.currentSceneState = SceneState.IN_GAME_SOLO;

        GD.Print("tag" + instance.currentScene.Name);
        //note code below could be made into seprate func for code reusability
        instance.currentScene.QueueFree();
        GD.Print("tag" + instance.currentScene.Name);
        instance.scene = ResourceLoader.Load<PackedScene>("res://Scenes/CormacShopGen.tscn");
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.GetChild(0).AddChild(newScene);
        instance.currentScene = newScene;
        //instance.pauseMenu = instance.GetNode<PauseMenu>("CanvasLayer2/PauseMenu");
        GD.Print("tag" + instance.currentScene.Name);


    }


    static public void OnDuoStart()
    {

        GD.Print("yes" + instance.currentScene.Name);

        instance.currentSceneState = SceneState.IN_GAME_DUO;


        //note code below could be made into seprate func for code reusability
        instance.currentScene.QueueFree();
        instance.scene = ResourceLoader.Load<PackedScene>("res://SplitScreenScene.tscn");
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.GetChild(0).AddChild(newScene);
        instance.currentScene = newScene;
        //instance.pauseMenu = instance.GetNode<PauseMenu>("CanvasLayer2/PauseMenu");
        //instance.shopGenerator = GetNode<ShopGenerator>("SubViewportContainer1/SubViewport1/TexNearest/ShopGenerator");

    }

    static public void OnMainMenuTransition()
    {
        instance.Playerlist.Clear();

        instance.currentSceneState = SceneState.MAIN_MENU;
        instance.currentScene.QueueFree();
        instance.scene = ResourceLoader.Load<PackedScene>(instance.MAIN_MENU_SCENE);
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.GetChild(0).AddChild(newScene);
        instance.currentScene = newScene;

    }

    static void onNewday(int quota, int mapHeight, int MapWidth,int itemsPerCatigory)
    {
        instance.dayTimer.Start();

        instance.dayNum++;
        if (instance.currentSceneState==SceneState.IN_GAME_DUO) 
        {

        }
        else if(instance.currentSceneState == SceneState.IN_GAME_SOLO)
        {
            
        }
    }
}
