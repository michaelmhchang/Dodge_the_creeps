using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	// Export is used for adjusting Speed's properties in the Godot IDE
	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec),
	public Vector2 ScreenSize; // Size of the game window

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;

	  	Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// The player's move vector
		// Vector is initialized to (0, 0), i.e. player is not moving
		var velocity = Vector2.Zero; 

		if (Input.IsActionPressed("move_right")) 
		{
			velocity.X += 1;
		}
		if (Input.IsActionPressed("move_left")) 
		{
			velocity.X -= 1;
		}
		if (Input.IsActionPressed("move_up")) 
		{
			velocity.Y -= 1;
		}
		if (Input.IsActionPressed("move_down")) 
		{
			velocity.Y += 1;
		}


		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if(velocity.Length() > 0)
		{
			// Normalizing the velocity prevents faster diagonal speed
			// -> moving 1 in X unit and 1 in Y unit is slower than moving (1, 1) diagonally due to shorter distance
			// thus normalizing and setting length to 1 makes all directions speed the same 
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		// delta is amount of time for one frame to complete, multiplying by delta insures consistent movement
		// regardless of framerate. Also velocity * time = distance
		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);


		// ANIMATION
		// ---------------------------------------------------------------------------  	
		if (velocity.X != 0) 
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false; // Do not flip vertically
			animatedSprite2D.FlipH = velocity.X < 0; // Flip horizontally when velocity in X direction is < 0
		}
		else if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}

	}

	private void OnBodyEntered(Node2D body)
	{
		Hide(); // Player disappears after getting hit
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback
		// Also may cause error to disable the player in the middle of a collision so deferred is used to wait until collision is finished
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	// Call to reset player when new game is started
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
}
