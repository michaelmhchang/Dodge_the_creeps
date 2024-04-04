using Godot;
using System;

public partial class Mob : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames(); // Gets all the animation names
		animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]); // Selects a random animation from index [0, 2)
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Deletes mobs when they hit edge of screen
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}
