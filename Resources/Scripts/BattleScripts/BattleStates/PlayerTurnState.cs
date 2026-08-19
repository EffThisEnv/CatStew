using Godot;
using System;

public partial class PlayerTurnState : BaseBattleState
{
	public PlayerTurnState(BattleManager manager) : base(manager)
	{

	}

	public override void EnterState()
	{
		GD.Print("Player Turn State");
		battleManager.ActionExecutorPicker.Visible = true;
		battleManager.ActionExecutorPicker.Clear();
		foreach (var character in battleManager.ActiveCharacters.Values)
		{
			battleManager.ActionExecutorPicker.AddItem(character.CharacterName, character.CharacterId - 1);
		}
	}

	public override void ExitState()
	{

	}

	public override void Update(double delta)
	{

	}
}
