using System;

[Flags]
public enum StaminaUsingMechanics
{
    None = 0,
    Sprint = 1,
    Jump = 2,
    Slide = 4,
    Dash = 8,
    WallRun = 16,
    WallJump = 32,
    WallClimb = 64
}