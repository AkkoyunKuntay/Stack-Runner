using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

namespace UI
{
    public class UIService : MonoBehaviour, IInitializable, IDisposable
    {
        [Header("Panel References")]
        [SerializeField] CanvasVisibilityController startPanel;
        [SerializeField] CanvasVisibilityController gamePanel;
        [SerializeField] CanvasVisibilityController winPanel;
        [SerializeField] CanvasVisibilityController failPanel;

        private readonly Dictionary<GameState, CanvasVisibilityController> _map =
            new Dictionary<GameState, CanvasVisibilityController>();

        [Inject] private IGameStateService _gameState;

        
        public void Initialize()
        {
            _map[GameState.None] = startPanel;
            _map[GameState.GamePlay] = gamePanel;
            _map[GameState.Success] = winPanel;
            _map[GameState.Failed] = failPanel;
 
            foreach (var p in _map.Values) p.Hide();
 
            HandleState(GameState.None, _gameState.currentState);
            _gameState.OnStateChangedEvent += HandleState;
        }

        public void Dispose() =>
            _gameState.OnStateChangedEvent -= HandleState;

       
        private void HandleState(GameState prev, GameState next)
        {
            if (_map.TryGetValue(prev, out var prevPanel)) prevPanel.Hide();
            if (_map.TryGetValue(next, out var nextPanel)) nextPanel.Show();
        }

        public void OnStartButton() => _gameState.SetState(GameState.GamePlay);
        public void OnRetryButton() => _gameState.Restart();          
        public void OnNextButton() => _gameState.SetState(GameState.GamePlay);
        public void OnQuitButton() => Application.Quit();
    }
}
