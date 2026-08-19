using Godot;
using System;

public partial class SetupState : BaseBattleState
{
	public SetupState(BattleManager manager) : base(manager)
	{
	}

	public override void EnterState()
	{
		if (base.battleManager.CurrentRound == 1)
		{
			BattleManager.EncounterPayload payload = new BattleManager.EncounterPayload
			{

				PlayerCount = 2,

				EnemyCount = 3

			};

			// Spawns characters based on the payload.
			base.battleManager.ProcessEncounter(payload);
		}
		base.battleManager.CurrentRound++;
		base.battleManager.GenerateTurnQueue();
		base.battleManager.TransitionToState(new CheckNextTurnState(base.battleManager));
	}

	public override void ExitState()
	{
	}

	public override void Update(double delta)
	{
	}
}