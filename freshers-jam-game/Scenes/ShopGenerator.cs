using Godot;
using Godot.Collections;
using System;

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
    Dictionary<Tile, Vector2I> tilesInMap;

    struct DirectionAllowance
    {
        public Tile[] Up, Down, Left, Right;
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
                if (x == 0 || y == 0 || y >= tiledata.GetLength(1) -1 || x >= tiledata.GetLength(0) -1)
                {
                    tiledata[x, y] = Tile.WALL;
                    tilePossibilities[x, y] = new Tile[1] { Tile.WALL };
                }
            }
        }

        // Initially calculate possibilities
        for (int x = 0; x < tiledata.GetLength(0); x++)
        {
            for (int y = 0; y < tiledata.GetLength(1); y++)
            {
                if (tiledata[x, y] == Tile.UNDEFINED){
                    Tile[] allowedTiles = new Tile[4] {Tile.WALL, Tile.FLOOR, Tile.SHELF_V, Tile.SHELF_H};

                }
            }
        }

        // WFC
        while (!IsMapFull()) { 
            // Check 
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
        GenerateNewMapData(11, 15);
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
}
