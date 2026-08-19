using Godot;
using System;
using System.Xml;
using System.Collections.Generic;

public partial class CharacterSpawner : Node3D
{
	// For complex UI, it's better to create a separate scene for the UI element (e.g., "CharUI.tscn")
	// and instantiate it here, rather than creating each node in code.
	[Export] private Characters PlayerResource, BasicEnemyResource, BasicFriendResource;

	[Export] public Characters.CharType CharacterTypeToSpawn { get; private set; }

	[Export] private CharacterSlot[] PlayerPositions;
	[Export] private CharacterSlot[] EnemyPositions;

	private Control _uiControlNode;

	[Export] Node3D EnemySide, PlayerSide;

	private int _idCounter = 0;

	public override void _Ready()
	{
		_uiControlNode = GetNode<Control>("../UI/Control");
	}

	public BattleCharacter InstantiateCharacter(Characters.CharType charType)
	{
		CharacterSlot availableSlot = GetAvailableSlot(charType);

		if (availableSlot == null)
		{
			GD.PrintErr($"No available slots for {charType} characters!");
			return null;
		}

		_idCounter++;
		BattleCharacter newChar = new BattleCharacter
		{
			Position = new Vector3(0, 3, 0)
		};

		var targetSide = (charType == Characters.CharType.Player || charType == Characters.CharType.Ally) ? PlayerSide : EnemySide;
		targetSide.AddChild(newChar);

		Characters selectedResource = charType switch
		{
			Characters.CharType.Player => PlayerResource,
			Characters.CharType.Ally => BasicFriendResource,
			Characters.CharType.Enemy => BasicEnemyResource,
			_ => BasicEnemyResource // Fallback
		};

		newChar.SetInitialValues(_idCounter, selectedResource);

		availableSlot.PlaceCharacter(newChar);
		newChar.GlobalPosition = availableSlot.GlobalPosition;

		SetupCharacterUI(newChar);

		return newChar;
	}
	//test test 

	private CharacterSlot GetAvailableSlot(Characters.CharType charType)
	{
		CharacterSlot[] slotsToSearch = (charType == Characters.CharType.Player || charType == Characters.CharType.Ally)
			? PlayerPositions
			: EnemyPositions;

		foreach (var slot in slotsToSearch)
		{
			if (!slot.IsPositionOccupied)
			{
				return slot;
			}
		}

		return null; // No slots available!
	}

	private void SetupCharacterUI(BattleCharacter newChar)
	{
		CharUI_Updater newCharUIContainer = new CharUI_Updater
		{
			Alignment = BoxContainer.AlignmentMode.Center,
			LayoutDirection = BoxContainer.LayoutDirectionEnum.Ltr,
			GrowVertical = BoxContainer.GrowDirection.Begin
		};
		_uiControlNode.AddChild(newCharUIContainer);

		ProgressBar progressBar = new ProgressBar
		{
			Value = newChar.CharacterCurrentHealth,
			MaxValue = newChar.CharacterMaxHealth,
			CustomMinimumSize = new Vector2(80, 15),
			MinValue = 0,
			ShowPercentage = false
		};
		newCharUIContainer.AddChild(progressBar);

		Label characterNameLabel = new Label
		{
			Text = newChar.CharacterName,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			GrowHorizontal = Control.GrowDirection.Both
		};
		newCharUIContainer.AddChild(characterNameLabel);

		newCharUIContainer.MoveChild(progressBar, 1); // Move progress bar below name
		newCharUIContainer.MoveChild(characterNameLabel, 0); // Keep name at top
		newCharUIContainer.AssignUIElements(characterNameLabel, progressBar);
		newCharUIContainer.OnCharacterValuesChanged(newChar); // Initial UI update
		newChar.OnCharacterValuesChanged += newCharUIContainer.OnCharacterValuesChanged;
		newChar.OnCharacterDeath += newCharUIContainer.OnCharacterDeath;
	}
}