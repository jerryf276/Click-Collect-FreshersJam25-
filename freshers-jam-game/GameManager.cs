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
        IN_GAME_DUO,
        CONTROLS
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
    SplitScreenManager splitScreenManager;
    MainMenu mainMenuScene;

    public bool newdaycreated = false;

    public int quota=2;
    public int currentProgress=0;
    public int dayNum=1;
    public int itemsPerCat=2;
    public int MapSize = 10;
    public int totalItemsDelivered = 0;
    public int totalDaysCompleted = 0;
    public int totalListsCompleted = 0;

    public List<Player> Playerlist = new List<Player>();

    SceneState currentSceneState;

    string MAIN_MENU_SCENE = "res://Scenes/TitleScreen.tscn";
    string CONTROLS_SCREEN = "res://Scenes/ControlsScreen.tscn";
    //  string SPLIT_SCREEN_PATH = "res://SplitScreenScene.tscn";

    public static List<Vector2> initialPlayerPositions;

    public static List<bool> positionReset;

    public override void _Ready()
    {
        instance = this;

        DisplayServer.WindowSetTitle("Click & collect");

        instance.dayTimer = instance.GetNode<Timer>("DayTime");
        instance.dayTimer.WaitTime = 100;

        currentSceneState = SceneState.MAIN_MENU;

        if (currentSceneState == SceneState.MAIN_MENU)
        {
            scene = ResourceLoader.Load<PackedScene>(MAIN_MENU_SCENE);
       //     mainMenuScene = ResourceLoader.Load<MainMenu>(MAIN_MENU_SCENE);
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

        initialPlayerPositions = new List<Vector2> { new Vector2(0, 0), new Vector2(0, 0) };
        positionReset = new List<bool> { false, false };
    }


    public override void _Process(double delta)
    {

        GD.Print("CURRENT PROGRESS: ", currentProgress);

        
        if (currentProgress==quota&&newdaycreated==false)
        {
            GD.Print("Day finished!");
            instance.newdaycreated = true;
            GD.Print("woah");
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
       // GD.Print("TIME LEFT: ", instance.dayTimer.TimeLeft);
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


        instance.dayTimer.Start();
       // instance.dayTimer.Paused = false;
        instance.currentSceneState = SceneState.IN_GAME_SOLO;

  
        //note code below could be made into seprate func for code reusability

        instance.currentScene.QueueFree();
        
        instance.scene = ResourceLoader.Load<PackedScene>("res://Scenes/CormacShopGen.tscn");
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.GetChild(0).AddChild(newScene);
        instance.currentScene = newScene;
        instance.shopGenerator = instance.GetNode<ShopGenerator>("CormacShopGen/ShopGenerator");

        
    }


    static public void OnDuoStart()
    {

        instance.dayTimer.Start();

        instance.currentSceneState = SceneState.IN_GAME_DUO;


        //note code below could be made into seprate func for code reusability
        instance.currentScene.QueueFree();
        instance.scene = ResourceLoader.Load<PackedScene>("res://SplitScreenScene.tscn");
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.GetChild(0).AddChild(newScene);
        instance.currentScene = newScene;
        instance.shopGenerator = instance.GetNode<ShopGenerator>("SplitScreenScene/SubViewportContainer1/SubViewport1/TexNearest/ShopGenerator");
        instance.splitScreenManager = instance.GetNode<SplitScreenManager>("SplitScreenScene");
        //instance.pauseMenu = instance.GetNode<PauseMenu>("CanvasLayer2/PauseMenu");
        //instance.shopGenerator = GetNode<ShopGenerator>("SubViewportContainer1/SubViewport1/TexNearest/ShopGenerator");

    }

    static public void OnMainMenuTransition()
    {
            Node newScene;
            instance.dayTimer.Stop();
            instance.Playerlist.Clear();
            instance.GetTree().Root.GetChild(0).RemoveChild(instance.currentScene);
            instance.currentSceneState = SceneState.MAIN_MENU;
            instance.currentScene.QueueFree();
            instance.scene = ResourceLoader.Load<PackedScene>(instance.MAIN_MENU_SCENE);
            newScene = instance.scene.Instantiate();
            instance.GetTree().Root.GetChild(0).AddChild(newScene);
            instance.currentScene = newScene;
           // instance.mainMenuScene = instance.GetNode<MainMenu>("TitleScreen");

    }

    static public void OnControlsStart()
    {
        
        //instance.GetTree().Root.GetChild(0).RemoveChild(instance.currentScene);
        instance.currentSceneState = SceneState.CONTROLS;
        instance.currentScene.QueueFree();
        instance.scene = ResourceLoader.Load<PackedScene>(instance.CONTROLS_SCREEN);
        Node newScene = instance.scene.Instantiate();
        instance.GetTree().Root.GetChild(0).AddChild(newScene);
        instance.currentScene = newScene;
    }

    static void onNewday(int quota, int mapHeight, int MapWidth,int itemsPerCatigory)
    {
        GD.Print("It's a new day.");
        instance.dayTimer.Start();
        instance.totalDaysCompleted++;
        instance.dayNum++;
        GD.Print(instance.dayNum, itemsPerCatigory, quota);

        if (instance.currentSceneState == SceneState.IN_GAME_DUO)
        {
            positionReset[0] = true;
            positionReset[1] = true;
        }
        //if (instance.currentSceneState == SceneState.IN_GAME_SOLO)
        //{
        //    // instance.Playerlist[0].Position = initialPlayerPositions[0];
        //    GetPlayers(0).Position = initialPlayerPositions[0];
        //}
        //else if (instance.currentSceneState == SceneState.IN_GAME_DUO)
        //{
        //    //instance.Playerlist[0].Position = initialPlayerPositions[0];
        //    //instance.Playerlist[1].Position = initialPlayerPositions[1];
        //    GetPlayers(0).Position = initialPlayerPositions[0];
        //    GetPlayers(1).Position = initialPlayerPositions[1];
        //}

        instance.shopGenerator.GenerateNewMapData(MapWidth, mapHeight);
        instance.shopGenerator.SetTilemapBasedOnData(itemsPerCatigory);
        instance.newdaycreated = false;

        
    }

    static public void CheckProg()
    {
       
        instance.currentProgress++;
        
    }

    //static public void IncrementDaysCompleted()
    //{
    //    instance.totalDaysCompleted++;
    //}

    static public void IncrementListsCompleted()
    {
        instance.totalListsCompleted++;
    }

    static public void AddItemsCompleted(int items)
    {
        instance.totalItemsDelivered += items;
    }


    static public int GetDaysCompleted()
    {
        return instance.totalDaysCompleted;
    }

    static public int GetListsCompleted()
    {
        return instance.totalListsCompleted;
    }

    static public int GetItemsDelivered()
    {
        return instance.totalItemsDelivered;
    }


    static public bool IsGameOver()
    {
        if (instance.dayTimer.TimeLeft <= 0.0f)
        {
           // GD.Print("TIME LEFT: ", instance.dayTimer.TimeLeft);
            return true;
        }
        return false;
    }

}
