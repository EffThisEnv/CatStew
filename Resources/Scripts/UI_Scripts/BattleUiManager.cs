using Godot;
using System;

public partial class BattleUiManager : Node
{
    public static event Action<object> onDisableAllUiRequest;
    public static event Action<object> onEnableAllUiRequest;


    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {

    }
}