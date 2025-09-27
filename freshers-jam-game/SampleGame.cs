using Godot;
using Godot.Collections;
using System;

public partial class SampleGame : Node
{
    private Dictionary<string, Dictionary<string, Node>> players;

    public override void _Ready()
    {
        players = new Dictionary<string, Dictionary<string, Node>>
        {
            {
                "1", new Dictionary<string, Node>
                {
                    { "viewport", GetNode<Viewport>("HBoxContainer/SubViewportContainer/SubViewport") },
                    { "camera", GetNode<Camera2D>("HBoxContainer/SubViewportContainer/SubViewport/Camera2D") },
                    { "player", GetNode<Node2D>("HBoxContainer/SubViewportContainer/SubViewport/Level/Player") }
                }
            },
            {
                "2", new Dictionary<string, Node>
                {
                    { "viewport", GetNode<Viewport>("HBoxContainer/SubViewportContainer2/SubViewport") },
                    { "camera", GetNode<Camera2D>("HBoxContainer/SubViewportContainer/SubViewport/Camera2D") },
                    { "player", GetNode<Node2D>("HBoxContainer/SubViewportContainer/SubViewport/Level/Player2") }
                }
            }
        };


        ((Viewport)players["2"]["viewport"]).World2D = ((Viewport)players["1"]["viewport"]).World2D;
    }
}
