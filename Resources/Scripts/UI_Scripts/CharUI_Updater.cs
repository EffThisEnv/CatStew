using Godot;
using System;

public partial class CharUI_Updater : VBoxContainer
{
    VBoxContainer _charUIContainer;
    Label _characterNameLabel;
    ProgressBar _progressBar;
    BattleCharacter _assignedCharacter;
    Camera3D camera;

    public override void _Ready()
    {
        camera = GetViewport().GetCamera3D();
    }

    public override void _Process(double delta)
    {
        if (_assignedCharacter != null && _charUIContainer != null && camera != null)
        {
            // Keep the UI element positioned over the 3D character in screen space.
            Vector3 target3DPosition = _assignedCharacter.GlobalPosition + new Vector3(0f, 3f, 0);
            Vector2 screenPosition = camera.UnprojectPosition(target3DPosition);
            screenPosition.X -= _charUIContainer.Size.X / 2;
            _charUIContainer.GlobalPosition = screenPosition;
        }
    }

    public void AssignUIElements(Label characterNameLabel, ProgressBar progressBar)
    {
        _charUIContainer = this;
        _characterNameLabel = characterNameLabel;
        _progressBar = progressBar;
    }

    public void UpdateUI()
    {
        if (_characterNameLabel != null && _assignedCharacter != null)
        {
            _characterNameLabel.Text = _assignedCharacter.CharacterName;
        }

        if (_progressBar != null && _assignedCharacter != null)
        {
            _progressBar.MaxValue = _assignedCharacter.CharacterMaxHealth;
            _progressBar.Value = _assignedCharacter.CharacterCurrentHealth;
            GD.Print($"Updated UI for {_assignedCharacter.CharacterName}: Health {_assignedCharacter.CharacterCurrentHealth}/{_assignedCharacter.CharacterMaxHealth}");
        }
    }

    public void OnCharacterValuesChanged(BattleCharacter character)
    {
        _assignedCharacter = character;
        UpdateUI();
    }

    public void OnCharacterDeath(BattleCharacter character)
    {
        if (_assignedCharacter == character)
        {
            _assignedCharacter = null;
            QueueFree();
        }
    }
}
