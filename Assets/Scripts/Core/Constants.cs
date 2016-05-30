using UnityEngine;

/// <summary>
/// List of constants used by the game.
/// </summary>
/// <remarks>
/// These values should not be tweaked readily.
/// Not to be initialized with config numbers.
/// Prevents magic numbers.
/// </remarks>
public static class Constants
{
    public const byte FoupSize = 25;
    public const int WaferDiameter = 300; // 300mm FAB

    public static Color BinColor(byte bin)
    {
        switch (bin)
        {
            case 0:
                return new Color(0.7f, 0.7f, 0.7f, 1f);
            case 1:
                return Color.green;
            case 3:
                return Color.red;
            case 5:
                return Color.blue;
            default:
                return Color.white;
        }
    }

    public static readonly Color SelectedColor = Color.black;
    public static readonly Color NotSelectedColor = Color.white;

    public static bool IsGoodDie(byte bin)
    {
        switch (bin)
        {
            case 0:
                return false;
            case 1:
                return true;
            default:
                return false;
        }
    }

    public enum UserInterface
    {
        Wafer,
        Lot
    }
}