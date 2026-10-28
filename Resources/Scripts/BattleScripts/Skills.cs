using Godot;
using System;

[GlobalClass]
public partial class Skills : Resource
{
    [Export] public string SkillName { get; set; }

    [Export] public Actions.ActionType Type { get; set; }

    [Export] public int Power { get; set; }

    [Export] public int CriticalMult { get; set; } = 1;

    [Export] public int TargetCount { get; set; } = 1;

    public enum TargetType
    {
        Self,
        Ally,
        Enemy,
        AllAlliesExceptSelf,
        AllAlliesAndSelf,
        AllEnemies,
        AllCharactersExceptSelf
    }

    [Export] public TargetType Target { get; set; }
}
