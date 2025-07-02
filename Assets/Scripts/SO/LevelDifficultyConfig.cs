using UnityEngine;

[CreateAssetMenu(menuName = "StackGame/Level Difficulty")]
public class LevelDifficultyConfig : ScriptableObject
{
    public string levelName = "Level 1";
    [Min(1)] public int normalPlatformCount = 10;
}