using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
public partial class BattleManager : Node
{
	// Manages the current phase of the battle.
	enum BattleState
	{
		Setup,          // Rolls initiative, builds the queue
		PlayerTurn,     // Turns on UI buttons, waits for player input
		EnemyTurn,      // Disables UI, asks AI to pick an action
		ExecutingAction,// (Your 'Animating' state) Locks the board while attacks/healing play out
		TurnCleanup,    // Checks if anyone died, removes them, advances the queue
		GameOver        // Victory or Defeat screens
	}
	private BattleState _currentState = BattleState.Setup;

	[Export] private CharacterSpawner characterSpawner;
	public Dictionary<int, BattleCharacter> ActiveCharacters = new Dictionary<int, BattleCharacter>();
	public List<int> TurnQueue { get; private set; } = new List<int>();

	Actions actions;

	public class EncounterPayload
	{
		public int PlayerCount;
		public int EnemyCount;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		actions = GetNode<Actions>("/root/Actions");
		GD.Print("Actions node found: " + (actions != null));
		// Manually calling _Ready() on another node is generally not recommended.
		characterSpawner._Ready();
		HandleSetupState();
	}

	private void HandleSetupState()
	{
		EncounterPayload payload = new EncounterPayload
		{
			PlayerCount = 2,
			EnemyCount = 3
		};

		// Spawns characters based on the payload.
		ProcessEncounter(payload);
		GenerateTurnQueue();
	}

	public void ProcessEncounter(BattleManager.EncounterPayload payload)
	{
		for (int i = 0; i < payload.PlayerCount; i++)
		{
			BattleCharacter temp = characterSpawner.InstantiateCharacter(Characters.CharType.Player);
			temp.OnCharacterDeath += (c) =>
			{
				// Remove the dead character from the active list and update the turn queue.
				ActiveCharacters.Remove(c.CharacterId);
				UpdateTurnQueue();
			};
			// Add the newly spawned character to our dictionary of active characters.
			ActiveCharacters.Add(temp.CharacterId, temp);
			GD.Print($"Spawned player skills are: {temp.CharacterMainSkill.SkillName}, {temp.CharacterSecondarySkill.SkillName}");
		}

		for (int i = 0; i < payload.EnemyCount; i++)
		{
			BattleCharacter temp = characterSpawner.InstantiateCharacter(Characters.CharType.Enemy);
			temp.OnCharacterDeath += (c) =>
			{
				ActiveCharacters.Remove(c.CharacterId);
				UpdateTurnQueue();
			};
			ActiveCharacters.Add(temp.CharacterId, temp);
		}
	}

	private void GenerateTurnQueue()
	{
		// Uses LINQ to order the character IDs by the Speed property of each character, in descending order.
		TurnQueue = ActiveCharacters.Keys.OrderByDescending(id => ActiveCharacters[id].CharacterSpeed).ToList();

		foreach (int id in TurnQueue)
		{
			GD.Print($"Queued ID {id} with Speed {ActiveCharacters[id].CharacterSpeed}");
		}
	}

	private void UpdateTurnQueue()
	{
		TurnQueue = TurnQueue.Where(id => ActiveCharacters.ContainsKey(id)).ToList();
		GD.Print("Queue updated!");
		foreach (int id in TurnQueue)
		{
			GD.Print($"Queued ID {id} with Speed {ActiveCharacters[id].CharacterSpeed}");
		}
	}

}
