using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
public partial class BattleManager : Node
{

	private BaseBattleState _currentState;
	public BattleCharacter CurrentPlayer;
	public int CurrentRound = 1;

	[Export] private CharacterSpawner characterSpawner;
	public Dictionary<int, BattleCharacter> ActiveCharacters = new Dictionary<int, BattleCharacter>();
	public List<int> TurnQueue { get; private set; } = new List<int>();

	Actions actions;

	[Export] public OptionButton ActionExecutorPicker, ActionPicker, TargetPicker;

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
		_currentState = new SetupState(this);
		_currentState.EnterState();
	}

	public override void _Process(double delta)
	{
		_currentState.Update(delta);
	}

	public void TransitionToState(BaseBattleState newState)
	{
		_currentState.ExitState();
		_currentState = newState;
		_currentState.EnterState();
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

	public void GenerateTurnQueue()
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
		// Simply filter out dead IDs without re-sorting or resetting the round
		TurnQueue = TurnQueue.Where(id => ActiveCharacters.ContainsKey(id)).ToList();
	}

}
