using Godot;
using System;

public partial class ShipHp : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdatePriceLabel();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Disabled = GlobalData.Instance.GetUpgradeShipHPPrice() > GlobalData.Instance.Resources;
	}

	public void BuyShipHPHandler()
	{
		GlobalData.Instance.Buy(GlobalData.Instance.GetUpgradeShipHPPrice());
		GlobalData.Instance.UpgradeShipHP();
		UpdatePriceLabel();
	}

	public void UpdatePriceLabel()
	{
		var priceLabel = GetNode<RichTextLabel>("Price");
		priceLabel.Text = GlobalData.Instance.GetUpgradeShipHPPrice().ToString();
	}
}
