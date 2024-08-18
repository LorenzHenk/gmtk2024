using Godot;
using System;

public partial class BuyTower : TextureButton
{


	[Export]
	public int Price;


	[Export]
	public PackedScene towerScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<RichTextLabel>("Price").Text = Price.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Disabled = !GlobalData.Instance.CanBuy(Price);
	}

	public void EnterPlacementMode()
	{
		var mousePosition = GetGlobalMousePosition();


		GetNode<Base>("/root/Base").EnablePlacementMode(Price, towerScene, mousePosition);

	}
}
