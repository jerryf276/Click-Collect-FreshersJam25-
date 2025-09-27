using Godot;
using System;

public partial class Player : Node2D
{
	private float playerSpeed = 250;
	[Export] private float maxPlayerSpeed = 250;
	[Export] private int playerNumber = 1;
	[Export] private CharacterBody2D player;
	private Vector2 lastDirection = new Vector2(1, 0);
	[Export] private int storedInventory = 0;
	private int maxInventoryCapacity = 5;

	public Shelf shelfImOn;

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

		switch (storedInventory)
		{
			case 0:
				playerSpeed = maxPlayerSpeed;
				break;
			case 1:
				playerSpeed = maxPlayerSpeed * 0.85f;
				break;
			case 2:
				playerSpeed = maxPlayerSpeed * 0.7f;
				break;
			case 3:
				playerSpeed = maxPlayerSpeed * 0.55f;
				break;
			case 4:
				playerSpeed = maxPlayerSpeed * 0.4f;
				break;
			case 5:
				playerSpeed = maxPlayerSpeed * 0.25f;
				break;
		}

		//if (inventorySize == 0)
		//{
		//    playerSpeed = 100;
		//}
		//else if (inventorySize == 1)
		//{
		//    playerSpeed = 80;
		//}

		//else if (inventorySize == 2)

		Vector2 velocity = player.Velocity;


		player.Velocity = direction * playerSpeed;
		player.MoveAndSlide();

		if (direction.Length() > 0)
		{
			lastDirection = direction;
		}
		
	   

	}
}
