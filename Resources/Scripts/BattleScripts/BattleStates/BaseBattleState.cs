using Godot;
using System;

public abstract class BaseBattleState
{
	protected BattleManager battleManager;

	public BaseBattleState(BattleManager manager)
	{
		battleManager = manager;
	}

	abstract public void EnterState();
	abstract public void ExitState();
	abstract public void Update(double delta);
}
