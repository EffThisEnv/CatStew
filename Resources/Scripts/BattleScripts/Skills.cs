using Godot;
using System;

[GlobalClass]
public partial class Skills : Resource
{
    [Export] public string SkillName { get; set; }

    [Export] public Actions.ActionType Category { get; set; }

    [Export] public int Power { get; set; }

    [Export] public int TargetCount { get; set; } = 1;
}
