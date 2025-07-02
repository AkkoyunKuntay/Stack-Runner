using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ILevelDifficultyService
{
    int CurrentLevelIndex { get; }
    int NormalPlatformCount { get; }
    event Action OnLevelAdvanced;              
    void NextLevel();
    void ResetAll();
    public LevelDifficultyConfig GetLevelData(int index);
}

public class LevelDifficultyService : ILevelDifficultyService
{
    [Inject] List<LevelDifficultyConfig> configs; 

    public int CurrentLevelIndex { get; private set; } = 0;
    public int NormalPlatformCount =>
        configs[Mathf.Clamp(CurrentLevelIndex, 0, configs.Count - 1)]
        .normalPlatformCount;

    public event Action OnLevelAdvanced;
    [Inject]
    public LevelDifficultyService()
    {
        Initialize();
    }
    public void Initialize() => Debug.Log($"[Difficulty] Level 0 started.");

    public void NextLevel()
    {
        CurrentLevelIndex = (CurrentLevelIndex + 1) % configs.Count;
        Debug.Log($"[Difficulty] Level {configs[CurrentLevelIndex].levelName} started.");
        OnLevelAdvanced?.Invoke();

    }

    public void ResetAll() => CurrentLevelIndex = 0;
    public LevelDifficultyConfig GetLevelData(int index)
    {
        return configs[index];
    }
}

