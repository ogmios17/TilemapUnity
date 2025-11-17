using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    private int currentLevel;
    public List<ScriptableLevel> levels;

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void NextLevel()
    {
        currentLevel++;
    }

    public void SetCurrentLevel(int index)
    {
        currentLevel = index;
    }
}
