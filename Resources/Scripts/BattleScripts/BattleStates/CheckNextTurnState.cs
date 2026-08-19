using Godot;
using System;

public partial class CheckNextTurnState : BaseBattleState
{
	public CheckNextTurnState(BattleManager manager) : base(manager)
	{
	}

	public override void EnterState()
	{
		if (battleManager.TurnQueue.Count == 0)
		{
			GD.PrintErr("Turn queue is empty.");
			return;
		}
		int currentId = battleManager.TurnQueue[0];

		if (battleManager.ActiveCharacters.TryGetValue(currentId, out BattleCharacter character))
		{
			base.battleManager.CurrentPlayer = character;

			if (character.CharacterType == Characters.CharType.Player)
			{
				battleManager.TransitionToState(new PlayerTurnState(battleManager));
			}
			else
			{
				battleManager.TransitionToState(new NPCTurnState(battleManager));
			}
		}
		else
		{
			// If the character somehow doesn't exist anymore, pop them and re-check
			//battleManager.TurnQueue.RemoveAt(0);
			//EnterState();
			GD.PrintErr($"Character with ID {currentId} not found in ActiveCharacters.");
		}
	}

	public override void ExitState()
	{

	}

	public override void Update(double delta)
	{
	}
}