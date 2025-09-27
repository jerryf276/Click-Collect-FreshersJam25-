using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ChecklistManager : Node
{
    public enum ChecklistItemCompletion { 
        NO, HELD, YES
    }

    [Export]
    public int maxItemsPerList = 4;
    [Export]
    public int minItemsPerList = 2;

    public HashSet<string> itemsInShop;

    public Dictionary<string, ChecklistItemCompletion> itemsInCurrentList = new Dictionary<string, ChecklistItemCompletion>();

    [Export]
    Area2D dropOffArea;

    [Export]
    Control checklistUI;

    int listsCompleted = 0;

    RandomNumberGenerator rng = new RandomNumberGenerator();

    Texture2D checkmarkTexture;

    public override void _Ready()
    {
        checkmarkTexture = ResourceLoader.Load<Texture2D>("res://Tiles/Checkmarks.png");

        dropOffArea.BodyEntered += (Node2D body) =>
        {
            if (body.GetParent() is Player) { 
                Player player = body.GetParent<Player>();

                foreach (string item in player.storedInventory) {
                    if (itemsInCurrentList.ContainsKey(item)) {
                        itemsInCurrentList[item] = ChecklistItemCompletion.YES;
                    }
                }

                player.storedInventory.Clear();
                player.storedInventoryNumber = 0;

                if(CheckListCompletion()) GenerateNewList();
            }
        };
    }

    public override void _Process(double delta)
    {
        foreach (KeyValuePair<string, ChecklistItemCompletion> kvp in itemsInCurrentList)
        {
            HBoxContainer container = checklistUI.GetNode<Control>("./Margin/VBox").GetNode<HBoxContainer>(kvp.Key);
            switch (kvp.Value) {
                case ChecklistItemCompletion.NO:
                    ((AtlasTexture)container.GetNode<TextureRect>("./Icon").Texture).Region = new Rect2(0, 32, 16, 16);
                    break;
                case ChecklistItemCompletion.HELD:
                    ((AtlasTexture)container.GetNode<TextureRect>("./Icon").Texture).Region = new Rect2(0, 16, 16, 16);
                    break;
                case ChecklistItemCompletion.YES:
                    ((AtlasTexture)container.GetNode<TextureRect>("./Icon").Texture).Region = new Rect2(0, 0, 16, 16);
                    break;
            }
        }
    }

    public void GenerateNewList() { 
        int numberOfItems = rng.RandiRange(minItemsPerList, maxItemsPerList);

        GD.Print(itemsInShop);

        while (itemsInCurrentList.Count < numberOfItems) { 
            int indexChosen = rng.RandiRange(0, itemsInShop.Count - 1);

            if(!itemsInCurrentList.ContainsKey(itemsInShop.ToArray()[indexChosen])) itemsInCurrentList.Add(itemsInShop.ToArray()[indexChosen], ChecklistItemCompletion.NO);
        }

        foreach (Node child in checklistUI.GetNode<Control>("./Margin/VBox").GetChildren()) child.Free();

        foreach (KeyValuePair<string, ChecklistItemCompletion> kvp in itemsInCurrentList)
        {
            // Create nodes programatically
            HBoxContainer container = new HBoxContainer();
            container.Name = kvp.Key;

            AtlasTexture checkmarkAtlas = new AtlasTexture();
            checkmarkAtlas.Atlas = checkmarkTexture;
            checkmarkAtlas.Region = new Rect2(0, 32, 16, 16);

            TextureRect checkImage = new TextureRect();
            checkImage.Name = "Icon";
            checkImage.Texture = checkmarkAtlas;
            checkImage.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;

            Godot.Label label = new Godot.Label();
            label.Text = kvp.Key;

            container.AddChild(checkImage);
            container.AddChild(label);

            checklistUI.GetNode<Control>("./Margin/VBox").AddChild(container);
        }
    }

    private bool CheckListCompletion() {
        foreach (KeyValuePair<string, ChecklistItemCompletion> kvp in itemsInCurrentList) { 
            if(kvp.Value != ChecklistItemCompletion.YES) return false;
        }
        listsCompleted++;
        itemsInCurrentList.Clear();
        return true;
    }
}
