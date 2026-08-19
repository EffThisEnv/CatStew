using Godot;
using System;
using System.Linq;

public partial class TurnCleanupState : BaseBattleState
{
	public TurnCleanupState(BattleManager manager) : base(manager)
	{

	}

	public override void EnterState()
	{
		base.battleManager.TurnQueue.Remove(base.battleManager.CurrentPlayer.CharacterId);

		// 1. Check Win/Loss conditions
		bool playersAlive = base.battleManager.ActiveCharacters.Values.Any(c => c.CharacterType == Characters.CharType.Player);
		bool enemiesAlive = base.battleManager.ActiveCharacters.Values.Any(c => c.CharacterType == Characters.CharType.Enemy);

		if (!enemiesAlive || !playersAlive)
		{
			GD.Print("Battle Over! Transitioning to BattleOverState. playersAlive: " + playersAlive + ", enemiesAlive: " + enemiesAlive);
			base.battleManager.TransitionToState(new BattleOverState(base.battleManager));
			return;
		}

		if (base.battleManager.TurnQueue.Count > 0)
		{
			base.battleManager.TransitionToState(new CheckNextTurnState(base.battleManager));
		}
		else
		{
			base.battleManager.TransitionToState(new SetupState(base.battleManager));
		}
	}

	public override void ExitState()
	{

	}


	public override void Update(double delta)
	{

	}
}
