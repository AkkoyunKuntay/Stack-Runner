using UnityEngine;
using Zenject;

public enum PanelTypes { Start, game, win, fail }
public class UIService : MonoBehaviour, IInitializable
{
    [Header("Panel References")]
    [SerializeField] private CanvasVisibilityController startPanel;
    [SerializeField] private CanvasVisibilityController gamePanel;
    [SerializeField] private CanvasVisibilityController winPanel;
    [SerializeField] private CanvasVisibilityController failPanel;

    [Header("Debug")]
    [SerializeField] CanvasVisibilityController activePanel;

    [Inject] private IGameFlowService _gameFlowService;
    [Inject] private ILevelDifficultyService _difficultyService;

    public void Initialize()
    {
        _gameFlowService.LevelStartedEvent += OnLevelStarted;
        _gameFlowService.LevelFailedEvent += OnLevelFailed;
        _gameFlowService.LevelSuccessEvent += OnLevelSuccessfull;

        Debug.Log("[UIService] has been initialized.");
    }

    public void OnStartButton() => _gameFlowService.StartGame();
    public void OnRetryButton() => _gameFlowService.RestartStage();
    public void OnNextButton() 
    {
        _difficultyService.NextLevel();
        _gameFlowService.NextStage();
    } 
    public void OnQuitButton() => Application.Quit();

    private void OnLevelStarted() => SetActivePanel(PanelTypes.game);
    private void OnLevelFailed() => SetActivePanel(PanelTypes.fail);
    private void OnLevelSuccessfull() => SetActivePanel(PanelTypes.win);

    private void SetActivePanel(PanelTypes type)
    {
        activePanel.Hide();
        switch (type)
        {
            case PanelTypes.Start:
                activePanel = startPanel;
                break;
            case PanelTypes.game:
                activePanel = gamePanel;
                break;
            case PanelTypes.win:
                activePanel = winPanel;
                break;
            case PanelTypes.fail:
                activePanel = failPanel;
                break;
            default:
                break;
        }
        activePanel.Show();
    }
}
