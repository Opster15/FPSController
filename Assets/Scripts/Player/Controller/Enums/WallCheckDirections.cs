
using System;

[Flags]
public enum WallCheckDirections
{
    None = 0,
    Forward = 1,
    Backward = 2,
    Left = 4,
    Right = 8
}
