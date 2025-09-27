using Godot;
using System;

public partial class SplitScreenManager : Node
{

    CharacterBody2D player1;
    CharacterBody2D player2;

    Viewport viewport1;
    Viewport viewport2;

    Camera2D camera1;
    Camera2D camera2;

    public override void _Ready()
    {
        player1 = GetNode<CharacterBody2D>("SubViewportContainer1/SubViewport1/Player1/CharacterBody2D");
        player2 = GetNode<CharacterBody2D>("SubViewportContainer2/SubViewport2/Player2/CharacterBody2D");

        viewport1 = GetNode<Viewport>("SubViewportContainer1/SubViewport1");
        viewport2 = GetNode<Viewport>("SubViewportContainer2/SubViewport2");

        camera1 = GetNode<Camera2D>("SubViewportContainer1/SubViewport1/Player1/Camera2D1");
        camera2 = GetNode<Camera2D>("SubViewportContainer2/SubViewport2/Player2/Camera2D2");

        viewport2.World2D = viewport1.World2D;
    }

    public override void _Process(double delta)
    {
        camera1.Position = player1.Position;
        camera2.Position = player2.Position;
    }

}
