using Godot;
using System;
using System.Xml.Linq;

public partial class Shelf : StaticBody2D
{
    public enum ShelfType { 
        FRIDGE, FREEZER, VEGTABLE, BREAD
    }

    [Export]
    public ShelfType myType;

    private string contains;

    [Export]
    public string Contains  
    {
        get { return contains; }  
        set { contains = value; GetNode<Godot.Label>("./InsideBubble/InsideMe").Text = value; } 
    }

    public override void _Ready()
    {
        GetNode<Area2D>("./VisibilityArea").BodyEntered += (Node2D body) =>
        {
            if (body.GetParent() is Player)
            {
                GetNode<Sprite2D>("./InsideBubble").Show();
                // TODO visually deal with overlap.
                body.GetParent<Player>().shelfImOn = this;
            }
        };
        GetNode<Area2D>("./VisibilityArea").BodyExited += (Node2D body) =>
        {
            if (body.GetParent() is Player)
            {
                GetNode<Sprite2D>("./InsideBubble").Hide();

                // Only remove if we are the shelf the player is on otherwise its some other shelf in the overlap
                if (body.GetParent<Player>().shelfImOn == this) body.GetParent<Player>().shelfImOn = null;
            }
        };
    }
}
