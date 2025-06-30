using Unity.VisualScripting;
using UnityEngine;
using System;
using Zenject;

public enum GameState { None, Start, GamePlay, Success, Failed }

public interface IGameStateService
{
    GameState currentState { get; }
    event Action<GameState, GameState> OnStateChangedEvent;
    void SetState(GameState newState);
    void Restart();
}

public class GameStateService : IGameStateService
{
    public GameState currentState { get; private set; } = GameState.None;
    public event Action<GameState, GameState> OnStateChangedEvent;

    [Inject]
    private GameStateService() 
    {
        Initialize();
    }
    public void Initialize() 
    {
        SetState(GameState.Start);
        Debug.Log("[GameStateService] has been initialized");
    } 

    public void SetState(GameState newState)
    {
        if (newState == currentState) return;

        var prev = currentState;
        currentState = newState;
        OnStateChangedEvent?.Invoke(prev, newState);
    }

    public void Restart()
    {
        currentState = GameState.None;
        SetState(GameState.Start);
    }
}







