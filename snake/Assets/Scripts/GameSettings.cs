using UnityEngine;

public static class GameSettings 
{
    public static DifficultySettings difficulty;

    public static string HighscoreKey(DifficultySettings difficulty) 
    {
        return $"Highscore_" + difficulty.name;
    }
}
