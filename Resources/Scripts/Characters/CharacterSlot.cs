using Godot;
using System;

public partial class CharacterSlot : Marker3D
{
	public bool IsPositionOccupied { get; private set; } = false;
	public BattleCharacter OccupyingCharacter { get; private set; }

	public void PlaceCharacter(BattleCharacter character)
	{
		if (!IsPositionOccupied)
		{
			OccupyingCharacter = character;
			IsPositionOccupied = true;
		}
		else
		{
			GD.Print("Position is already occupied!");
		}
	}
}
