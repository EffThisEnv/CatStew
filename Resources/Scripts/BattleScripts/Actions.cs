using Godot;
using System;
using System.Linq;

public partial class Actions : Node
{
	public enum ActionType
	{
		Attack,
		Heal,
		Buff,
		Stun
	}

	// Central dispatcher for all character actions.
	public void ExecuteAction(ActionType actionType, BattleCharacter source, BattleCharacter[] target, int criticalChance = 0, int damage = 0, int healAmount = 0, int buffAmount = 0, int stunDuration = 0)
	{
		switch (actionType)
		{
			case ActionType.Attack:
				ExecuteAttack(source, target, damage, criticalChance);
				break;
			case ActionType.Heal:
				ExecuteHeal(source, target, healAmount);
				break;
			case ActionType.Buff:
				ExecuteBuff(source, target, buffAmount);
				break;
			case ActionType.Stun:
				ExecuteStun(source, target, stunDuration);
				break;
			default:
				// This is good practice for ensuring all enum values are handled.
				throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
		}
	}

	public void ExecuteAttack(BattleCharacter Attacker, BattleCharacter[] targets, int damage, int criticalChance)
	{
		bool isCritical = GD.RandRange(0, 100) < criticalChance;
		int finalDamage = isCritical ? damage * 2 : damage;

		foreach (BattleCharacter target in targets)
		{
			target.SubtractValue(finalDamage);
		}

		GD.Print($"{Attacker.CharacterName} attacked {string.Join(", ", targets.Select(t => t.CharacterName))} for {finalDamage} damage. Critical Hit: {isCritical}");
	}

	public void ExecuteHeal(BattleCharacter Healer, BattleCharacter[] targets, int healAmount)
	{
		foreach (BattleCharacter target in targets)
		{
			// Note: This could potentially overheal. A future improvement could be to cap healing at max health.
			target.SetValue(target.CharacterCurrentHealth + healAmount, target.CharacterCurrentAttackDamage, target.CharacterCoinCount);
		}

		GD.Print($"{Healer.CharacterName} healed {string.Join(", ", targets.Select(t => t.CharacterName))} for {healAmount} health.");
	}

	public void ExecuteBuff(BattleCharacter BuffGiver, BattleCharacter[] targets, int buffAmount)
	{
		foreach (BattleCharacter target in targets)
		{
			target.SetValue(target.CharacterCurrentHealth, target.CharacterCurrentAttackDamage + buffAmount, target.CharacterCoinCount);
		}

		GD.Print($"{BuffGiver.CharacterName} buffed {string.Join(", ", targets.Select(t => t.CharacterName))} for {buffAmount} attack damage.");
	}

	public void ExecuteStun(BattleCharacter Stunner, BattleCharacter[] targets, int stunDuration)
	{
		foreach (BattleCharacter target in targets)
		{
			// Implement stun logic here. For example, you might set a "stunned" state on the target.
			GD.Print($"{target.CharacterName} is stunned for {stunDuration} turns.");
		}

		GD.Print($"{Stunner.CharacterName} stunned {string.Join(", ", targets.Select(t => t.CharacterName))} for {stunDuration} turns.");
	}
}
