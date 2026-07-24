using Godot;
using System;

[GlobalClass]
public partial class Characters : Resource
{
	public enum CharType
	{
		Player,
		Ally,
		Enemy
	}

	[Export] public string CharacterName { get; set; }
	[Export] public CharType CharacterType { get; set; }
	[Export] public Mesh CharacterMesh { get; set; }
	[Export] public int CharacterMaxHealth { get; set; }
	[Export] public int CharacterMaxAttackDamage { get; set; }
	[Export] public int CharacterSpeed { get; set; }
	[Export] public int CharacterCoinCount { get; set; }

	[Export] public Skills MainSkill { get; set; }
	[Export] public Skills SecondarySkill { get; set; }
}