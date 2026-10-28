using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BattleUiManager : Node
{
    public static event Action OnDisableAllUiRequest;
    public static event Action OnEnableAllUiRequest;

    [Export] private Control _playerActionMenu;
    [Export] private OptionButton _actionExecutorPicker;
    [Export] private OptionButton _actionPicker;
    [Export] private OptionButton _targetPicker;
    [Export] private Button _executeActionButton;

    private BattleCharacter[] _availableTargets;
    private BattleCharacter[] _tempTargetContainer;
    private int _selectedCharacterId;

    Actions actions;

    public const int TARGET_ALL_ENEMIES = -100;
    public const int TARGET_ALL_ALLIES = -101;
    public const int TARGET_ALL_CHARACTERS = -102;

    public override void _Ready()
    {
        actions = GetNode<Actions>("/root/Actions");

        _executeActionButton.Pressed += _onExecuteActionButtonPressed;

        // _playerActionMenu.Visible = false;
        // _actionExecutorPicker.Clear();
        // _actionPicker.Clear();
        // _targetPicker.Clear();
    }
    public void OnActivePlayersReady(Dictionary<int, BattleCharacter> ActiveCharacters)
    {
        _actionExecutorPicker.Clear();
        _populateAvailableTargets(ActiveCharacters);

        _actionExecutorPicker.AddItem("Select Character", -1); // Placeholder for no selection

        foreach (var kvp in ActiveCharacters)
        {
            _actionExecutorPicker.AddItem(kvp.Value.CharacterName, kvp.Value.CharacterId - 1);
        }

        _actionExecutorPicker.Selected = 0; // Default to the first character

        _actionExecutorPicker.ItemSelected += (index) => _getCharacterIdFromOptionButton(index, ActiveCharacters);
        // GD.Print("Action Executor Picker populated with active players.");
    }

    private void _populateAvailableTargets(Dictionary<int, BattleCharacter> ActiveCharacters)
    {
        _availableTargets = ActiveCharacters.Values.ToArray();
    }

    public void EnableAllUI()
    {
        OnEnableAllUiRequest?.Invoke();

        var staticUiElements = GetTree().GetNodesInGroup("PlayerBattleUI");
        foreach (Node node in staticUiElements)
        {
            if (node is Control control)
            {
                control.Visible = true;
            }
        }
    }

    public void DisableAllUI()
    {
        OnDisableAllUiRequest?.Invoke();

        var staticUiElements = GetTree().GetNodesInGroup("PlayerBattleUI");
        foreach (Node node in staticUiElements)
        {
            if (node is Control control)
            {
                control.Visible = false;
            }
        }
    }


    private void _getCharacterIdFromOptionButton(long index, Dictionary<int, BattleCharacter> ActiveCharacters)
    {
        if (_actionExecutorPicker.GetItemText(0) == "Select Character")
        {
            _actionExecutorPicker.RemoveItem(0);
        }

        int selectedIndex = (int)index;
        if (selectedIndex >= 0 && selectedIndex < ActiveCharacters.Count)
        {
            int characterId = selectedIndex + 1; // Assuming character IDs start from 1
            _selectedCharacterId = characterId;
            _findCharacterById(characterId, ActiveCharacters);
        }
        else
        {
            GD.Print("Invalid selection in OptionButton.");
        }
    }

    private void _findCharacterById(int characterId, Dictionary<int, BattleCharacter> ActiveCharacters)
    {
        if (ActiveCharacters.TryGetValue(characterId, out BattleCharacter character))
        {
            _populateActionPicker(character);
            //GD.Print("Found Character: " + character.CharacterName);
        }
        else
        {
            // GD.Print("Character with ID " + characterId + " not found.");
        }
    }

    private void _populateActionPicker(BattleCharacter character)
    {
        _actionPicker.Clear();
        if (character.CharacterMainSkill != null)
        {
            _actionPicker.AddItem(character.CharacterMainSkill.SkillName, 0);
            _actionPicker.AddItem(character.CharacterSecondarySkill.SkillName, 1);
        }
        //GD.Print("Action Picker populated with available actions.");
        _actionPicker.Visible = true;
        _populateTargetPicker(_availableTargets);
    }

    private void _populateTargetPicker(BattleCharacter[] targets)
    {
        _targetPicker.Clear();
        foreach (var target in targets)
        {
            if (target.CharacterId != _selectedCharacterId)
            {
                _targetPicker.AddItem(target.CharacterName, id: target.CharacterId); // Assuming CharacterId starts from 1
            }
        }
        _targetPicker.AddItem("AllEnemies", id: TARGET_ALL_ENEMIES); // Add all enemies option
        _targetPicker.AddItem("AllAllies", id: TARGET_ALL_ALLIES); // Add all allies option
        _targetPicker.AddItem("AllCharactersExceptSelf", id: TARGET_ALL_CHARACTERS); // Add all characters option
        //GD.Print("Target Picker populated with available targets.");
        _targetPicker.Visible = true;
    }


    private void _onExecuteActionButtonPressed()
    {
        int actionIndex = (int)_actionPicker.Selected;
        int targetCharacterId = _targetPicker.GetSelectedId();

        if (actionIndex < 0 || targetCharacterId < 0)
        {
            GD.Print("Please select both an action and a target.");
            return;
        }

        BattleCharacter executor = _availableTargets.FirstOrDefault(c => c.CharacterId == _selectedCharacterId);
        BattleCharacter target = _availableTargets.FirstOrDefault(c => c.CharacterId == targetCharacterId);

        if (executor == null || target == null)
        {
            GD.Print("Executor or target not found.");
            return;
        }

        Skills selectedSkill = actionIndex == 0 ? executor.CharacterMainSkill : executor.CharacterSecondarySkill;

        if (selectedSkill != null)
        {
            actions.ExecuteAction(selectedSkill, executor, target, null, selectedSkill.Target);
            GD.Print($"Executed {selectedSkill.SkillName} from {executor.CharacterName} on {target.CharacterName}");
        }
        else
        {
            GD.Print("Selected skill is null.");
        }
    }

    private void _populateMultipleTargetsOptions(int id)
    {
        switch (id)
        {
            case TARGET_ALL_ENEMIES:
                _tempTargetContainer = _availableTargets
                    .Where(target => target.CharacterType == Characters.CharType.Enemy)
                    .ToArray();

                break;

            case TARGET_ALL_ALLIES:
                _tempTargetContainer = _availableTargets
                    .Where(target => target.CharacterType == Characters.CharType.Ally)
                    .ToArray();
                break;

            case TARGET_ALL_CHARACTERS:
                _tempTargetContainer = _availableTargets
                    .Where(target => target.CharacterType != Characters.CharType.Player)
                    .ToArray();
                break;
        }
    }

    public void _repopulateAllOptionButtons(Dictionary<int, BattleCharacter> ActiveCharacters)
    {
        _actionExecutorPicker.Clear();
        _actionExecutorPicker.AddItem("Select Character", -1); // Placeholder for no selection

        DisableAllUI();

        _populateAvailableTargets(ActiveCharacters);
        foreach (var kvp in ActiveCharacters)
        {
            _actionExecutorPicker.AddItem(kvp.Value.CharacterName, kvp.Value.CharacterId - 1);
        }

        _actionPicker.Clear();
        _actionPicker.Visible = false; // Disable until a character is selected
        _targetPicker.Clear();
        _targetPicker.Visible = false;

        _actionExecutorPicker.Selected = 0; // Default to the first character

        EnableAllUI();

        GD.Print("Action Executor Picker repopulated with active players.");
    }
}