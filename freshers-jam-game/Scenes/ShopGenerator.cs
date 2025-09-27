using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[Tool]
public partial class ShopGenerator : Node
{
    [ExportToolButton("Regenerate Tile Map")]
    public Callable ClickMeButton => Callable.From(FullRefreshTilemap);

    [Export]
    TileMapLayer tilemap;

    enum Tile { 
      UNDEFINED,
      FLOOR,
      WALL,
      FRIDGE_H,
      FRIDGE_V,
      FREEZER_H,
      FREEZER_V,
      VEGTABLE_H,
      VEGTABLE_V,
      BREAD_H,
      BREAD_V
    };

    [Export]
    Godot.Collections.Dictionary<Tile, Vector2I> tilesInMap;

    // Tool for returning values which appear in both arrays. 
    static private Tile[] ReturnCrossoverValues(Tile[] a, Tile[] b)
    {
        HashSet<Tile> hashA = new HashSet<Tile>(a);
        HashSet<Tile> hashB = new HashSet<Tile>(b);

        for (int i = 0; i < a.Length; i++) {
            if (!hashB.Contains(a[i])) hashA.Remove(a[i]);
        }

        return hashA.ToArray();
    }

    // Direction allowance struct
    struct DirectionAllowance
    {
        public Tile[] Up, Down, Left, Right;
        public Tile[] GetDirectionOpposite(Vector2I direction) {
            if (direction == new Vector2I(1, 0)) return Left;
            else if (direction == new Vector2I(-1, 0)) return Right;
            else if (direction == new Vector2I(0, 1)) return Up;
            else if (direction == new Vector2I(0, -1)) return Down;
            else if (direction == new Vector2I(-1, -1)) return ReturnCrossoverValues(Down, Right);
            else if (direction == new Vector2I(1, -1)) return ReturnCrossoverValues(Down, Left);
            else if (direction == new Vector2I(-1, 1)) return ReturnCrossoverValues(Up, Right);
            else if (direction == new Vector2I(1, 1)) return ReturnCrossoverValues(Up, Left);
            return System.Array.Empty<Tile>();
        }
    }

    
    // List of allowed tiles, diagonally it must match say Up and Right rules. 
    System.Collections.Generic.Dictionary<Tile, DirectionAllowance> allowedTiles = new System.Collections.Generic.Dictionary<Tile, DirectionAllowance>{
        { Tile.FLOOR, new DirectionAllowance{
            Up = new Tile[10]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.FREEZER_V, Tile.VEGTABLE_H, Tile.VEGTABLE_V, Tile.BREAD_H, Tile.BREAD_V},
            Down = new Tile[10]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.FREEZER_V, Tile.VEGTABLE_H, Tile.VEGTABLE_V, Tile.BREAD_H, Tile.BREAD_V },
            Left = new Tile[10] { Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.FREEZER_V, Tile.VEGTABLE_H, Tile.VEGTABLE_V, Tile.BREAD_H, Tile.BREAD_V },
            Right = new Tile[10] { Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.FREEZER_V, Tile.VEGTABLE_H, Tile.VEGTABLE_V, Tile.BREAD_H, Tile.BREAD_V }
        } },
        { Tile.WALL, new DirectionAllowance{
            Up = new Tile[6]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FREEZER_V, Tile.VEGTABLE_V, Tile.BREAD_V},
            Down = new Tile[6]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FREEZER_V, Tile.VEGTABLE_V, Tile.BREAD_V},
            Left = new Tile[6]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.VEGTABLE_H, Tile.BREAD_H},
            Right = new Tile[6]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.VEGTABLE_H, Tile.BREAD_H }
        } },
        { Tile.FRIDGE_V, new DirectionAllowance{
            Up = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FREEZER_V},
            Down = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FREEZER_V},
            Left = new Tile[1]{ Tile.FLOOR},
            Right = new Tile[1]{ Tile.FLOOR}
        } },
        { Tile.FRIDGE_H, new DirectionAllowance{
            Up = new Tile[1]{ Tile.FLOOR},
            Down = new Tile[1]{ Tile.FLOOR},
            Left = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_H, Tile.FREEZER_H},
            Right = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_H, Tile.FREEZER_H}
        } }
        ,
        { Tile.FREEZER_V, new DirectionAllowance{
            Up = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FREEZER_V},
            Down = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FREEZER_V},
            Left = new Tile[1]{ Tile.FLOOR},
            Right = new Tile[1]{ Tile.FLOOR}
        } },
        { Tile.FREEZER_H, new DirectionAllowance{
            Up = new Tile[1]{ Tile.FLOOR},
            Down = new Tile[1]{ Tile.FLOOR},
            Left = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_H, Tile.FREEZER_H},
            Right = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.FRIDGE_H, Tile.FREEZER_H}
        } },
        { Tile.VEGTABLE_V, new DirectionAllowance{
            Up = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_V, Tile.BREAD_V},
            Down = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_V, Tile.BREAD_V},
            Left = new Tile[1]{ Tile.FLOOR},
            Right = new Tile[1]{ Tile.FLOOR}
        } },
        { Tile.VEGTABLE_H, new DirectionAllowance{
            Up = new Tile[1]{ Tile.FLOOR},
            Down = new Tile[1]{ Tile.FLOOR},
            Left = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_H, Tile.BREAD_H},
            Right = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_H, Tile.BREAD_H }
        } },
        { Tile.BREAD_V, new DirectionAllowance{
            Up = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_V, Tile.BREAD_V},
            Down = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_V, Tile.BREAD_V},
            Left = new Tile[1]{ Tile.FLOOR},
            Right = new Tile[1]{ Tile.FLOOR}
        } },
        { Tile.BREAD_H, new DirectionAllowance{
            Up = new Tile[1]{ Tile.FLOOR},
            Down = new Tile[1]{ Tile.FLOOR},
            Left = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_H, Tile.BREAD_H},
            Right = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.VEGTABLE_H, Tile.BREAD_H }
        } }
    };

    Tile[,] tiledata;

    Tile[,][] tilePossibilities;

    RandomNumberGenerator rng = new RandomNumberGenerator();

    // For items inside shelves
    const string ITEM_DICT_FILE = "res://ItemDictionary.txt";
    System.Collections.Generic.Dictionary<Shelf.ShelfType, List<string>> itemsByShelfType = new System.Collections.Generic.Dictionary<Shelf.ShelfType, List<string>>();

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;

        ReloadItemDictionary();

        FullRefreshTilemap();
    }

    public override void _Process(double delta)
    {
    }

    public void GenerateNewMapData(int width, int height) {
        tiledata = new Tile[width + 2, height + 2];
        tilePossibilities = new Tile[width + 2, height + 2][];

        // Add walls
        for (int x = 0; x < tiledata.GetLength(0); x++)
        {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                if (x == 0 || y == 0 || y >= tiledata.GetLength(1) - 1 || x >= tiledata.GetLength(0) - 1)
                {
                    tiledata[x, y] = Tile.WALL;
                    tilePossibilities[x, y] = new Tile[0];
                }
                else {
                    tilePossibilities[x, y] = new Tile[10] { Tile.FLOOR, Tile.WALL, Tile.FRIDGE_V, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.FREEZER_V, Tile.VEGTABLE_H, Tile.VEGTABLE_V, Tile.BREAD_H, Tile.BREAD_V };
                }

                // Very specifically overwrite 0,2 to be a floor
                if (x == 0 && y == 2) {
                    tiledata[x, y] = Tile.FLOOR;
                    tilePossibilities[x, y] = new Tile[0];
                }
            }
        }
        GD.Print("Check all map");
        for (int x = 0; x < tiledata.GetLength(0); x++)
        {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                CalculateTilePossibilites(new Vector2I(x, y), null, false);
            }
        }
        GD.Print("Loop Begin");
        // WFC
        int hangPrevention = 0;
        while (!IsMapFull()) {
            hangPrevention++;
            if (hangPrevention > 1500) {
                GD.PrintErr("I've gone on too long...");
                return;
            }
            // Check 
            Vector2I colapsed = ColapseLeastPossibilities();
            if (colapsed.X == -1)
            {
                GD.PrintErr("Its impossible...");
                return;
            }
            // Propogate
            if (colapsed.Y - 1 >= 0) CalculateTilePossibilites(colapsed + new Vector2I(0, -1), System.Array.Empty<Vector2I>());
            if (colapsed.Y + 1 < tiledata.GetLength(1)) CalculateTilePossibilites(colapsed + new Vector2I(0, 1), System.Array.Empty<Vector2I>());
            if (colapsed.X - 1 >= 0) CalculateTilePossibilites(colapsed + new Vector2I(-1, 0), System.Array.Empty<Vector2I>());
            if (colapsed.X + 1 < tiledata.GetLength(0)) CalculateTilePossibilites(colapsed + new Vector2I(1, 0), System.Array.Empty<Vector2I>());
        }
    }

    public void SetTilemapBasedOnData(int maxItemsPerCategory = 3) {
        for (int x = 0; x < tiledata.GetLength(0); x++)
        {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                tilemap.SetCell(new Vector2I(x, y), -1);
            }
        }
        for (int x = 0; x < tiledata.GetLength(0); x++) {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                tilemap.SetCell(new Vector2I(x, y), 1, Vector2I.Zero, tilesInMap[tiledata[x, y]].X);
                Callable.From(() =>
                {
                    if (!Engine.IsEditorHint())
                    {
                        // Create dictionary of already chosen items
                        System.Collections.Generic.Dictionary<Shelf.ShelfType, HashSet<string>> chosenItems = new System.Collections.Generic.Dictionary<Shelf.ShelfType, HashSet<string>>();

                        for (int i = 0; i < tilemap.GetChildren().Count; i++)
                        {
                            if (tilemap.GetChildOrNull<Shelf>(i) != null)
                            {
                                Shelf thisShelf = tilemap.GetChild<Shelf>(i);
                                if (itemsByShelfType[thisShelf.myType] != null && itemsByShelfType[thisShelf.myType].Count > 0)
                                {
                                    if (!chosenItems.ContainsKey(thisShelf.myType)) chosenItems.Add(thisShelf.myType, new HashSet<string>());

                                    // Use an item from chosen items if we are full
                                    if (chosenItems[thisShelf.myType].Count >= maxItemsPerCategory)
                                    {
                                        thisShelf.Contains = chosenItems[thisShelf.myType].ToArray()[rng.RandiRange(0, chosenItems[thisShelf.myType].Count - 1)];
                                    }
                                    // Otherwise use an item from the dictionary and add it to our chosen items
                                    else
                                    {
                                        thisShelf.Contains = itemsByShelfType[thisShelf.myType][rng.RandiRange(0, itemsByShelfType[thisShelf.myType].Count - 1)];
                                        chosenItems[thisShelf.myType].Add(thisShelf.Contains);
                                    }
                                }
                            }
                        }
                    }
                }).CallDeferred();
            }
        }
    }

    private void FullRefreshTilemap() {
        GenerateNewMapData(7, 8);
        SetTilemapBasedOnData(2);
    }


    private bool IsMapFull() {
        for (int x = 0; x < tiledata.GetLength(0); x++)
        {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                if (tiledata[x, y] == Tile.UNDEFINED) return false;
            }
        }
        return true;
    }

    private void CalculateTilePossibilites(Vector2I on, Vector2I[] beenTo, bool recurse = true) {
        HashSet<Tile> currentPossibilities = new HashSet<Tile>() { Tile.FLOOR, Tile.FRIDGE_V, Tile.FRIDGE_H, Tile.FREEZER_H, Tile.FREEZER_V, Tile.VEGTABLE_H, Tile.VEGTABLE_V, Tile.BREAD_H, Tile.BREAD_V };
        //GD.Print("Now checking: ", on);
        // Dont check me if I am already colapsed
        if (tiledata[on.X, on.Y] == Tile.UNDEFINED)
        {
            /*
            // Check above
            if (on.Y - 1 >= 0)
            {
                HashSet<Tile> possibilitiesThisDirection = new HashSet<Tile>();
                if (tiledata[on.X, on.Y - 1] == Tile.UNDEFINED)
                {
                    foreach (Tile tile in tilePossibilities[on.X, on.Y - 1])
                    {
                        foreach (Tile allowedTile in allowedTiles[tile].Down)
                        {
                            possibilitiesThisDirection.Add(allowedTile);
                        }
                    }
                }
                else
                {
                    foreach (Tile allowedTile in allowedTiles[tiledata[on.X, on.Y - 1]].Down)
                    {
                        possibilitiesThisDirection.Add(allowedTile);
                    }
                }
                foreach (Tile overallAllowed in currentPossibilities)
                {
                    bool included = false;
                    foreach (Tile allowed in possibilitiesThisDirection)
                    {
                        if (allowed == overallAllowed)
                        {
                            included = true; break;
                        }
                    }
                    if (!included) currentPossibilities.Remove(overallAllowed);
                }
            }
            // Check below
            if (on.Y + 1 < tilePossibilities.GetLength(1))
            {
                HashSet<Tile> possibilitiesThisDirection = new HashSet<Tile>();
                if (tiledata[on.X, on.Y + 1] == Tile.UNDEFINED)
                {
                    foreach (Tile tile in tilePossibilities[on.X, on.Y + 1])
                    {
                        foreach (Tile allowedTile in allowedTiles[tile].Up)
                        {
                            possibilitiesThisDirection.Add(allowedTile);
                        }
                    }
                }
                else
                {
                    foreach (Tile allowedTile in allowedTiles[tiledata[on.X, on.Y + 1]].Up)
                    {
                        possibilitiesThisDirection.Add(allowedTile);
                    }
                }
                foreach (Tile overallAllowed in currentPossibilities)
                {
                    bool included = false;
                    foreach (Tile allowed in possibilitiesThisDirection)
                    {
                        if (allowed == overallAllowed)
                        {
                            included = true; break;
                        }
                    }
                    if (!included) currentPossibilities.Remove(overallAllowed);
                }
            }
            // Check left
            if (on.X - 1 >= 0)
            {
                HashSet<Tile> possibilitiesThisDirection = new HashSet<Tile>();
                if (tiledata[on.X - 1, on.Y] == Tile.UNDEFINED)
                {
                    foreach (Tile tile in tilePossibilities[on.X - 1, on.Y])
                    {
                        foreach (Tile allowedTile in allowedTiles[tile].Right)
                        {
                            possibilitiesThisDirection.Add(allowedTile);
                        }
                    }
                }
                else
                {
                    foreach (Tile allowedTile in allowedTiles[tiledata[on.X - 1, on.Y]].Right)
                    {
                        possibilitiesThisDirection.Add(allowedTile);
                    }
                }
                foreach (Tile overallAllowed in currentPossibilities)
                {
                    bool included = false;
                    foreach (Tile allowed in possibilitiesThisDirection)
                    {
                        if (allowed == overallAllowed)
                        {
                            included = true; break;
                        }
                    }
                    if (!included) currentPossibilities.Remove(overallAllowed);
                }
            }
            // Check right
            if (on.X + 1 < tilePossibilities.GetLength(0))
            {
                HashSet<Tile> possibilitiesThisDirection = new HashSet<Tile>();
                if (tiledata[on.X + 1, on.Y] == Tile.UNDEFINED)
                {
                    foreach (Tile tile in tilePossibilities[on.X + 1, on.Y])
                    {
                        foreach (Tile allowedTile in allowedTiles[tile].Left)
                        {
                            possibilitiesThisDirection.Add(allowedTile);
                        }
                    }
                }
                else
                {
                    foreach (Tile allowedTile in allowedTiles[tiledata[on.X + 1, on.Y]].Left)
                    {
                        possibilitiesThisDirection.Add(allowedTile);
                    }
                }
                foreach (Tile overallAllowed in currentPossibilities)
                {
                    bool included = false;
                    foreach (Tile allowed in possibilitiesThisDirection)
                    {
                        if (allowed == overallAllowed)
                        {
                            included = true; break;
                        }
                    }
                    if (!included) currentPossibilities.Remove(overallAllowed);
                }
            }*/

            // Check above
            if (on.Y - 1 >= 0)
            {
                UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(0, -1));
                if (on.X + 1 < tilePossibilities.GetLength(0))
                {
                    UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(1, -1));
                }
            }
            // Check below
            if (on.Y + 1 < tilePossibilities.GetLength(1))
            {
                UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(0, 1));
                if (on.X - 1 >= 0)
                {
                    UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(-1, 1));
                }
            }
            // Check left
            if (on.X - 1 >= 0)
            {
                UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(-1, 0));
                if (on.Y - 1 >= 0)
                {
                    UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(-1, -1));
                }
            }
            // Check right
            if (on.X + 1 < tilePossibilities.GetLength(0))
            {
                UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(1, 0));
                if (on.Y + 1 < tilePossibilities.GetLength(1))
                {
                    UpdateCurrentPosibilities(ref currentPossibilities, on, on + new Vector2I(1, 1));
                }
            }

            // Time to check my neighbours if I have changed
            if (recurse && !currentPossibilities.SetEquals(new HashSet<Tile>(tilePossibilities[on.X, on.Y])))
            {
                if (tiledata[on.X, on.Y] == Tile.UNDEFINED) tilePossibilities[on.X, on.Y] = currentPossibilities.ToArray();
                if (on.Y - 1 >= 0 && !beenTo.Contains(new Vector2I(on.X, on.Y - 1)))
                {
                    Vector2I[] passIn = new Vector2I[beenTo.Length + 1];
                    passIn[beenTo.Length] = on;
                    //GD.Print("Going Deeper Up!");
                    CalculateTilePossibilites(new Vector2I(on.X, on.Y - 1), passIn);
                
                }
                if (on.Y + 1 < tilePossibilities.GetLength(1) && !beenTo.Contains(new Vector2I(on.X, on.Y + 1)))
                {
                    Vector2I[] passIn = new Vector2I[beenTo.Length + 1];
                    passIn[beenTo.Length] = on;
                    //GD.Print("Going Deeper Down!");
                    CalculateTilePossibilites(new Vector2I(on.X, on.Y + 1), passIn);
                
                }
                if (on.X - 1 >= 0 && !beenTo.Contains(new Vector2I(on.X - 1, on.Y)))
                {
                    Vector2I[] passIn = new Vector2I[beenTo.Length + 1];
                    passIn[beenTo.Length] = on;
                    //GD.Print("Going Deeper Left!");
                    CalculateTilePossibilites(new Vector2I(on.X - 1, on.Y), passIn);
                
                }
                if (on.X + 1 < tilePossibilities.GetLength(0) && !beenTo.Contains(new Vector2I(on.X + 1, on.Y)))
                {
                    Vector2I[] passIn = new Vector2I[beenTo.Length + 1];
                    passIn[beenTo.Length] = on;
                    //GD.Print("Going Deeper Right!");
                    CalculateTilePossibilites(new Vector2I(on.X + 1, on.Y), passIn);
                
                }
            }
            else {
                if (tiledata[on.X, on.Y] == Tile.UNDEFINED) tilePossibilities[on.X, on.Y] = currentPossibilities.ToArray();
                //GD.Print("Finished Now!");
            }
        }
    }

    private void UpdateCurrentPosibilities(ref HashSet<Tile> posibilities, Vector2I from, Vector2I check) {
        HashSet<Tile> possibilitiesThisDirection = new HashSet<Tile>();
        if (tiledata[check.X, check.Y] == Tile.UNDEFINED)
        {
            foreach (Tile tile in tilePossibilities[check.X, check.Y])
            {
                foreach (Tile allowedTile in allowedTiles[tile].GetDirectionOpposite(check - from))
                {
                    possibilitiesThisDirection.Add(allowedTile);
                }
            }
        }
        else
        {
            foreach (Tile allowedTile in allowedTiles[tiledata[check.X, check.Y]].GetDirectionOpposite(check - from))
            {
                possibilitiesThisDirection.Add(allowedTile);
            }
        }
        foreach (Tile overallAllowed in posibilities)
        {
            bool included = false;
            foreach (Tile allowed in possibilitiesThisDirection)
            {
                if (allowed == overallAllowed)
                {
                    included = true; break;
                }
            }
            if (!included) posibilities.Remove(overallAllowed);
        }
    }

    private Vector2I ColapseLeastPossibilities() {
        // Not gonna random select lowest, lets try top left first?/!
        Vector2I lowestAt = new Vector2I(0, 0);
        int lowest = int.MaxValue;
        for (int x = 0; x < tiledata.GetLength(0); x++)
        {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                if (tilePossibilities[x, y].Length < lowest && tiledata[x, y] == Tile.UNDEFINED) {
                    
                    lowest = tilePossibilities[x, y].Length;
                    lowestAt = new Vector2I(x, y);
                    GD.Print("Tile lower: ", lowest, lowestAt);
                }
            }
        }

        // Now colapse it
        if (tilePossibilities[lowestAt.X, lowestAt.Y].Length > 0)
        {
            GD.Print("Colapsing: ", lowestAt);
            int chosenColapse = rng.RandiRange(0, tilePossibilities[lowestAt.X, lowestAt.Y].Length - 1);
            tiledata[lowestAt.X, lowestAt.Y] = tilePossibilities[lowestAt.X, lowestAt.Y][chosenColapse];
            tilePossibilities[lowestAt.X, lowestAt.Y] = System.Array.Empty<Tile>();
            return lowestAt;
        }
        else {
            return new Vector2I(-1, -1); // ERROR
        }
    }

    public void ReloadItemDictionary() {
        using var file = Godot.FileAccess.Open(ITEM_DICT_FILE, Godot.FileAccess.ModeFlags.Read);
        string jsondata = file.GetAsText();
        Json json = new Json();
        json.Parse(jsondata);

        itemsByShelfType.Clear();

        foreach (Variant typeAndItemPair in (Array<Variant>)json.Data) {
            string[] typeAndItem = typeAndItemPair.AsStringArray();
            if (!itemsByShelfType.ContainsKey((Shelf.ShelfType)typeAndItem[0].ToInt())) itemsByShelfType.Add((Shelf.ShelfType)typeAndItem[0].ToInt(), new List<string>());
            if (itemsByShelfType[(Shelf.ShelfType)typeAndItem[0].ToInt()] == null) itemsByShelfType[(Shelf.ShelfType)(typeAndItem[0].ToInt())] = new List<string>();
            itemsByShelfType[(Shelf.ShelfType)(typeAndItem[0].ToInt())].Add(typeAndItem[1]);
            GD.Print(typeAndItem);
        }
    }
}
