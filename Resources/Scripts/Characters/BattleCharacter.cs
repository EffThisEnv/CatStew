using Godot;
using System;
using System.Linq;

public partial class BattleCharacter : MeshInstance3D
{
	public int CharacterId { get; private set; }
	public string CharacterName { get; private set; }
	public Mesh CharacterMesh { get; private set; }
	public Characters.CharType CharacterType;

	public int CharacterMaxHealth { get; private set; }
	public int CharacterCurrentHealth { get; set; }

	public int CharacterSpeed { get; set; }

	public int CharacterMaxAttackDamage { get; set; }
	public int CharacterCurrentAttackDamage { get; set; }
	public int CharacterCoinCount { get; set; }

	public Skills CharacterMainSkill { get; private set; }
	public Skills CharacterSecondarySkill { get; private set; }

	public event Action<BattleCharacter> OnCharacterValuesChanged;
	public event Action<BattleCharacter> OnCharacterDeath;


	public void SetInitialValues(int UniqueId, Characters characterResource)
	{
		CharacterId = UniqueId;
		CharacterName = characterResource.CharacterName;
		CharacterMesh = characterResource.CharacterMesh;
		CharacterType = characterResource.CharacterType;
		this.Mesh = CharacterMesh;
		CharacterMaxHealth = characterResource.CharacterMaxHealth;
		CharacterMaxAttackDamage = characterResource.CharacterMaxAttackDamage;
		CharacterSpeed = characterResource.CharacterSpeed;
		CharacterMainSkill = characterResource.MainSkill;
		CharacterSecondarySkill = characterResource.SecondarySkill;

		SetValue(characterResource.CharacterMaxHealth, characterResource.CharacterMaxAttackDamage, characterResource.CharacterCoinCount);
	}

	public void SetValue(int newHealth, int newAttackDamage, int newCoinCount)
	{
		CharacterCurrentHealth = newHealth;
		CharacterCurrentAttackDamage = newAttackDamage;
		CharacterCoinCount = newCoinCount;
		OnCharacterValuesChanged?.Invoke(this);
	}

	public void SubtractValue(int HealthValue = 0, int AttackValue = 0, int CoinValue = 0)
	{
		// Don't process if already dead.
		if (CharacterCurrentHealth <= 0) return;

		CharacterCurrentHealth -= HealthValue;
		CharacterCurrentAttackDamage -= AttackValue;
		CharacterCoinCount -= CoinValue;
		OnCharacterValuesChanged?.Invoke(this);

		// Check for death after values change.
		CheckDeath();
	}

	private bool CheckDeath()
	{
		if (CharacterCurrentHealth <= 0)
		{
			CharacterDeath();
			return true;
		}
		return false;
	}

	public void CharacterDeath()
	{
		GD.Print($"{CharacterName} has died.");
		OnCharacterDeath?.Invoke(this);
		RemoveAllListeners();
		this.QueueFree();
		GD.Print("Character disposed");
	}

	public void RemoveAllListeners()
	{
		OnCharacterValuesChanged = null;
	}
}
