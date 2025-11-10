using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Player : Node2D
{
	private float playerSpeed = 250;
	[Export] private float maxPlayerSpeed = 250;
	[Export] public int playerNumber = 1;
	[Export] private CharacterBody2D player;
	private Vector2 lastDirection = new Vector2(1, 0);
	private int maxInventoryCapacity = 5;
	private AudioStreamPlayer sfxCollect;

	public HashSet<string> storedInventory = new HashSet<string>();
    [Export] public int storedInventoryNumber = 0;

    public Shelf shelfImOn;

	private AnimationPlayer playerAnimation;
	private Sprite2D playerSprite;
	private Texture2D player2Texture;
	bool initialPositionSet;

	public override void _Ready()
	{
		GameManager.AddtoPlayer(this);
		sfxCollect = GetNode<AudioStreamPlayer>("sfx_collect");
		playerAnimation = GetNode<AnimationPlayer>("CharacterBody2D/Sprite2D/AnimationPlayer");
		playerSprite = GetNode<Sprite2D>("CharacterBody2D/Sprite2D");
		player2Texture = GD.Load<Texture2D>("res://Sprites/thumbnail_Karen_WalkingRight2.png");

		if (playerNumber == 2)
		{
			playerSprite.Texture = player2Texture;
		}
		initialPositionSet = false;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (initialPositionSet == false)
		{
            if (playerNumber == 1)
			{
                GameManager.initialPlayerPositions[0] = new Vector2(Position.X, Position.Y);
            }
			if (playerNumber == 2)
			{
                GameManager.initialPlayerPositions[1] = new Vector2(Position.X, Position.Y);
            }
			initialPositionSet = true;
        }
		//if (playerNumber == 1)
		//{
		var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		//}
		if (playerNumber == 2)
		{
			direction = Input.GetVector("move_left_p2", "move_right_p2", "move_up_p2", "move_down_p2");
		}

		switch (storedInventoryNumber)
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
		//playerSprite.Texture = ResourceLoader.Load("");


		player.Velocity = direction * playerSpeed;
		player.MoveAndSlide();

		if (direction.Length() > 0)
		{
			lastDirection = direction;
		}

		bool soundPlayed = false;

		for (int i = 0; i < player.GetSlideCollisionCount(); i++)
		{
			var collision = player.GetSlideCollision(i);
			//GD.Print("I collided with", ((Node)collision.GetCollider()).Name);

			if (playerNumber == 1)
			{
				if (Input.IsActionPressed("pickup"))
				{
					if (storedInventoryNumber < maxInventoryCapacity)
					{
						if ((Node)collision.GetCollider() is Shelf)
						{
							shelfImOn = (Shelf)collision.GetCollider();
							GD.Print("I collided with ", shelfImOn.Contains);
							storedInventory.Add(shelfImOn.Contains);
							storedInventoryNumber = storedInventory.Count;
							GD.Print(storedInventoryNumber);
							if (soundPlayed == false)
							{
								sfxCollect.Play();
								soundPlayed = true;
							}
						}
					}
				}
			}

			else if (playerNumber == 2)
			{

				if (Input.IsActionPressed("pickup_p2"))
				{
					if (storedInventoryNumber < maxInventoryCapacity)
					{
						if ((Node)collision.GetCollider() is Shelf)
						{
							shelfImOn = (Shelf)collision.GetCollider();
							GD.Print("I collided with ", shelfImOn.Contains);
							storedInventory.Add(shelfImOn.Contains);
							storedInventoryNumber = storedInventory.Count;
							GD.Print(storedInventoryNumber);
                            if (soundPlayed == false)
                            {
                                sfxCollect.Play();
                                soundPlayed = true;
                            }
                        }
					}
                }
			}
		}

		if (player.Velocity.X > 0)
		{
			playerSprite.FlipH = false;
		}
		else if (player.Velocity.X < 0)
		{
			playerSprite.FlipH = true;
		}


		if (player.Velocity.X != 0 || player.Velocity.Y != 0)
		{
			playerAnimation.Play("walkRight");
		}
		else
		{
			playerAnimation.Stop();
		}
	}
}

//	private void OnAreaEntered(Area2D area)
//	{
//		if (area is Shelf)
//		{
//			if (playerNumber == 1)
//			{
//                if (Input.IsActionPressed("pickup"))
//				{
//					//if (shelfImOn.)
//					//Adds item to inventory
//					storedInventory.Add(shelfImOn.Contains);
//				}

//            }

//			else
//			{
//				if (Input.IsActionPressed("pickup_p2"))
//				{
//					storedInventory.Add(shelfImOn.Contains);
//				}
//			}
//		}
//	}
//}
