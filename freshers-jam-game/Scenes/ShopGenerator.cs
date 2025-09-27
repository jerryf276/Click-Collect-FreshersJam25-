using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
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
      SHELF_H,
      SHELF_V
    };

    [Export]
    Godot.Collections.Dictionary<Tile, Vector2I> tilesInMap;

    static private Tile[] ReturnCrossoverValues(Tile[] a, Tile[] b)
    {
        HashSet<Tile> result = new HashSet<Tile>(a);
        for (int i = 0; i < a.Length; i++)
        {
            bool included = false;
            for (int j = 0; j < b.Length; j++)
            {
                if (a[i] == b[j])
                {
                    included = true; break;
                }
            }
            if (!included) result.Remove(a[i]);
        }
        return result.ToArray();
    }

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

    

    System.Collections.Generic.Dictionary<Tile, DirectionAllowance> allowedTiles = new System.Collections.Generic.Dictionary<Tile, DirectionAllowance>{
        { Tile.FLOOR, new DirectionAllowance{
            Up = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V, Tile.SHELF_H},
            Down = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V, Tile.SHELF_H},
            Left = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V, Tile.SHELF_H},
            Right = new Tile[4]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V, Tile.SHELF_H}
        } },
        { Tile.WALL, new DirectionAllowance{
            Up = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V},
            Down = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V},
            Left = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_H},
            Right = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_H}
        } },
        { Tile.SHELF_V, new DirectionAllowance{
            Up = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V},
            Down = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_V},
            Left = new Tile[1]{ Tile.FLOOR},
            Right = new Tile[1]{ Tile.FLOOR}
        } },
        { Tile.SHELF_H, new DirectionAllowance{
            Up = new Tile[1]{ Tile.FLOOR},
            Down = new Tile[1]{ Tile.FLOOR},
            Left = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_H},
            Right = new Tile[3]{ Tile.FLOOR, Tile.WALL, Tile.SHELF_H }
        } }
    };

    Tile[,] tiledata;

    Tile[,][] tilePossibilities;

    RandomNumberGenerator rng = new RandomNumberGenerator();
    public override void _Ready()
    {
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
                    tilePossibilities[x, y] = new Tile[3] { Tile.FLOOR, Tile.SHELF_V, Tile.SHELF_H };
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

    public void SetTilemapBasedOnData() {
        for (int x = 0; x < tiledata.GetLength(0); x++) {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                tilemap.SetCell(new Vector2I(x, y), 21, tilesInMap[tiledata[x, y]]);
            }
        }
    }

    private void FullRefreshTilemap() {
        GenerateNewMapData(30, 30);
        SetTilemapBasedOnData();
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
        HashSet<Tile> currentPossibilities = new HashSet<Tile>() { Tile.FLOOR, Tile.SHELF_V, Tile.SHELF_H };
        GD.Print("Now checking: ", on);
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
                    GD.Print("Going Deeper Up!");
                    CalculateTilePossibilites(new Vector2I(on.X, on.Y - 1), passIn);
                
                }
                if (on.Y + 1 < tilePossibilities.GetLength(1) && !beenTo.Contains(new Vector2I(on.X, on.Y + 1)))
                {
                    Vector2I[] passIn = new Vector2I[beenTo.Length + 1];
                    passIn[beenTo.Length] = on;
                    GD.Print("Going Deeper Down!");
                    CalculateTilePossibilites(new Vector2I(on.X, on.Y + 1), passIn);
                
                }
                if (on.X - 1 >= 0 && !beenTo.Contains(new Vector2I(on.X - 1, on.Y)))
                {
                    Vector2I[] passIn = new Vector2I[beenTo.Length + 1];
                    passIn[beenTo.Length] = on;
                    GD.Print("Going Deeper Left!");
                    CalculateTilePossibilites(new Vector2I(on.X - 1, on.Y), passIn);
                
                }
                if (on.X + 1 < tilePossibilities.GetLength(0) && !beenTo.Contains(new Vector2I(on.X + 1, on.Y)))
                {
                    Vector2I[] passIn = new Vector2I[beenTo.Length + 1];
                    passIn[beenTo.Length] = on;
                    GD.Print("Going Deeper Right!");
                    CalculateTilePossibilites(new Vector2I(on.X + 1, on.Y), passIn);
                
                }
            }
            else {
                if (tiledata[on.X, on.Y] == Tile.UNDEFINED) tilePossibilities[on.X, on.Y] = currentPossibilities.ToArray();
                GD.Print("Finished Now!");
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
}
