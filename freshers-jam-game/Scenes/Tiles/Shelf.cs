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

    [Export]
    Godot.Label whatIContainLabel;

    private string contains;

    [Export]
    public string Contains  
    {
        get { return contains; }  
        set { contains = value; if (whatIContainLabel != null) {
                whatIContainLabel.Text = value;
                whatIContainLabel.Scale = new Vector2(0.2f, 0.2f);
                whatIContainLabel.Position = new Vector2(-whatIContainLabel.Size.X / 10.0f, 0); // Its ten because its 0.2 scale and we need half of that.
                
            } } 
    }

    public override void _Ready()
    {
        GetNode<Area2D>("./VisibilityArea").BodyEntered += (Node2D body) =>
        {
            if (body.GetParent() is Player)
            {
                GetNode<Sprite2D>("./InsideBubble").Show();
            }
        };
        GetNode<Area2D>("./VisibilityArea").BodyExited += (Node2D body) =>
        {
            if (body.GetParent() is Player)
            {
                GetNode<Sprite2D>("./InsideBubble").Hide();
            }
        };
    }
}
