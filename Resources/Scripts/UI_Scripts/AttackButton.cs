using Godot;
using System;

public partial class AttackButton : Button
{
	// NOTE: Using hardcoded paths like "../../../Environment" can be fragile.
	// A better solution is to use an [Export] NodePath or an Autoload singleton.
	private BattleManager battleManager;

	public override void _Ready()
	{
		this.Pressed += AttackFunction;
		battleManager = GetNode<BattleManager>("../../../Environment");
	}

	private void AttackFunction()
	{
		// This is a temporary implementation for testing.
		// It's hardcoded to attack the character with ID 1.
		// This should be made dynamic to allow player target selection.
		if (battleManager.ActiveCharacters.ContainsKey(1))
		{
			GD.Print("Attack button pressed, dealing 20 damage to Character ID 1.");
			// TODO: This logic should go through the BattleManager/Actions system, not directly modify the character.
			// e.g., battleManager.PlayerPerformAction(skill, target);
			battleManager.ActiveCharacters[1].SubtractValue(20, 0, 0);
		}
		else
		{
			GD.Print("Attack button pressed, but Character ID 1 not found!");
		}
	}
}
