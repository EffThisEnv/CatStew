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
	public void ExecuteAction(Skills skill, BattleCharacter source, BattleCharacter target, BattleCharacter[] targets, Skills.TargetType targetType)
	{
		BattleCharacter[] validTargets = targets ?? (target != null ? new BattleCharacter[] { target } : Array.Empty<BattleCharacter>());

		// Pass source into validation so relationships (Self, Ally, Enemy) are known
		if (CheckTargetValidity(source, validTargets, targetType))
		{
			switch (skill.Type)
			{
				case ActionType.Attack:
					ExecuteAttack(source, validTargets, skill.Power, skill.CriticalMult);
					break;
				case ActionType.Heal:
					ExecuteHeal(source, target, validTargets, skill.Power);
					break;
				case ActionType.Buff:
					ExecuteBuff(source, target, validTargets, skill.Power);
					break;
				case ActionType.Stun:
					ExecuteStun(source, target, validTargets, skill.Power);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(skill.Type), skill.Type, null);
			}
		}
	}

	public void ExecuteAttack(BattleCharacter Attacker, BattleCharacter[] targets, int damage, int criticalChance)
	{
		bool isCritical = GD.RandRange(0, 100) < criticalChance;
		int finalDamage = isCritical ? damage * 2 : damage;

		targets ??= Array.Empty<BattleCharacter>();
		foreach (BattleCharacter character in targets)
		{
			character.SubtractValue(finalDamage);
		}

		GD.Print($"{Attacker.CharacterName} attacked {string.Join(", ", targets.Select(t => t.CharacterName))} for {finalDamage} damage. Critical Hit: {isCritical}");
	}

	public void ExecuteHeal(BattleCharacter Healer, BattleCharacter target, BattleCharacter[] targets, int healAmount)
	{
		targets ??= (target != null ? new BattleCharacter[] { target } : Array.Empty<BattleCharacter>());
		foreach (BattleCharacter character in targets)
		{
			character.SetValue(character.CharacterCurrentHealth + healAmount, character.CharacterCurrentAttackDamage, character.CharacterCoinCount);
		}

		GD.Print($"{Healer.CharacterName} healed {string.Join(", ", targets.Select(t => t.CharacterName))} for {healAmount} health.");
	}

	public void ExecuteBuff(BattleCharacter BuffGiver, BattleCharacter target, BattleCharacter[] targets, int buffAmount)
	{
		targets ??= (target != null ? new BattleCharacter[] { target } : Array.Empty<BattleCharacter>());
		foreach (BattleCharacter character in targets)
		{
			character.SetValue(character.CharacterCurrentHealth, character.CharacterCurrentAttackDamage + buffAmount, character.CharacterCoinCount);
		}

		GD.Print($"{BuffGiver.CharacterName} buffed {string.Join(", ", targets.Select(t => t.CharacterName))} for {buffAmount} attack damage.");
	}

	public void ExecuteStun(BattleCharacter Stunner, BattleCharacter target, BattleCharacter[] targets, int stunDuration)
	{
		targets ??= (target != null ? new BattleCharacter[] { target } : Array.Empty<BattleCharacter>());
		foreach (BattleCharacter character in targets)
		{
			character.ApplyStun(stunDuration);
			GD.Print($"{character.CharacterName} is stunned for {stunDuration} turns.");
		}

		GD.Print($"{Stunner.CharacterName} stunned {string.Join(", ", targets.Select(t => t.CharacterName))} for {stunDuration} turns.");
	}

	private static bool CheckTargetValidity(BattleCharacter source, BattleCharacter[] targets, Skills.TargetType targetType)
	{
		if (targets == null || targets.Length == 0)
		{
			GD.PrintErr("Target validation failed: No targets provided.");
			return false;
		}

		// Helper: Players and Allies are on the same team; Enemies are on the opposing team
		bool IsAlly(BattleCharacter a, BattleCharacter b)
		{
			bool aIsPlayerSide = a.CharacterType != Characters.CharType.Enemy;
			bool bIsPlayerSide = b.CharacterType != Characters.CharType.Enemy;
			return aIsPlayerSide == bIsPlayerSide;
		}

		switch (targetType)
		{
			case Skills.TargetType.Self:
				if (targets.Length != 1)
				{
					GD.PrintErr("Target validation failed: Self-targeting skills can only target exactly one unit.");
					return false;
				}
				if (targets[0].CharacterId != source.CharacterId)
				{
					GD.PrintErr($"Target validation failed: {targets[0].CharacterName} is not the executor ({source.CharacterName}).");
					return false;
				}
				return true;

			case Skills.TargetType.Enemy:
				if (targets.Length != 1)
				{
					GD.PrintErr("Target validation failed: Single-enemy skills require exactly one target.");
					return false;
				}
				if (IsAlly(source, targets[0]))
				{
					GD.PrintErr($"Target validation failed: Cannot target ally/friendly unit ({targets[0].CharacterName}) with an enemy-only skill.");
					return false;
				}
				return true;

			case Skills.TargetType.Ally:
				if (targets.Length != 1)
				{
					GD.PrintErr("Target validation failed: Single-ally skills require exactly one target.");
					return false;
				}
				if (!IsAlly(source, targets[0]) || targets[0].CharacterId == source.CharacterId)
				{
					GD.PrintErr($"Target validation failed: {targets[0].CharacterName} is not an ally (or is the caster).");
					return false;
				}
				return true;

			case Skills.TargetType.AllEnemies:
				foreach (BattleCharacter t in targets)
				{
					if (IsAlly(source, t))
					{
						GD.PrintErr($"Target validation failed: Target list contains a friendly unit ({t.CharacterName}) for an All-Enemies skill.");
						return false;
					}
				}
				return true;

			case Skills.TargetType.AllAlliesAndSelf:
				foreach (BattleCharacter t in targets)
				{
					if (!IsAlly(source, t))
					{
						GD.PrintErr($"Target validation failed: Target list contains an enemy unit ({t.CharacterName}) for an ally-wide skill.");
						return false;
					}
				}
				return true;

			case Skills.TargetType.AllAlliesExceptSelf:
				foreach (BattleCharacter t in targets)
				{
					if (!IsAlly(source, t) || t.CharacterId == source.CharacterId)
					{
						GD.PrintErr($"Target validation failed: Invalid unit ({t.CharacterName}) for an all-allies-except-self skill.");
						return false;
					}
				}
				return true;

			case Skills.TargetType.AllCharactersExceptSelf:
				return true;

			default:
				GD.PrintErr($"Target validation failed: Unhandled target type {targetType}.");
				return false;
		}
	}
}