using Godot;
using System;

public partial class Player : Node2D
{
    private float playerSpeed = 100;
    [Export] private int playerNumber = 1;
    [Export] private CharacterBody2D player;
    private Vector2 lastDirection = new Vector2(1, 0);
    public override void _Ready()
    {

    }
    public override void _PhysicsProcess(double delta)
    {
        //if (playerNumber == 1)
        //{
            var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        //}
        if (playerNumber == 2)
        {
            direction = Input.GetVector("move_left_p2", "move_right_p2", "move_up_p2", "move_down_p2");   
        }

        Vector2 velocity = player.Velocity;


        player.Velocity = direction * playerSpeed;
        player.MoveAndSlide();

        if (direction.Length() > 0)
        {
            lastDirection = direction;
        }
        
       

    }
}
